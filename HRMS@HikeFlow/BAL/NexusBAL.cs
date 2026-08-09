using HRMS_Common;
using HRMS_Common.CommonProperties;
using HRMS_HikeFlow.DAL;
using HRMS_HikeFlow.NexusProperties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using static HRMS_Common.General;
using System.Text.Json;
using System.Net;

namespace HRMS_HikeFlow.NexusBAL
{
    public class NexusBAL
    {
        private AppDb _objAppDb { get; }
        private readonly RegularExpressions? _objRegularExpressions;
        private readonly General _objGeneral;

        public NexusBAL(AppDb objAppDb, General? objGeneral = null, RegularExpressions? objRegularExpressions = null)
        {
            _objAppDb = objAppDb;
            _objGeneral = objGeneral ?? new General();
            _objRegularExpressions = objRegularExpressions;
        }


        /// <summary>
        /// Hashes the user's new password using BCrypt and updates their credentials in the database.
        /// </summary>
        /// <param name="objAPIRequest">Payload containing the target user ID, new plain-text password, and request IP address.</param>
        /// <returns>A Response object indicating the outcome of the password change request.</returns>
        public Response<object> ChangePassword(ChangePasswordRequestDto objAPIRequest)
        {

            DateTime dtStartResponseTime = DateTime.Now;
            int intErrorCode = 0;

            Response<object>? objResponse = null;
            int intResult = 0;

            try
            {
                // Hash the new password using BCrypt before sending it to the database
                if (objAPIRequest != null && !string.IsNullOrWhiteSpace(objAPIRequest.NewPassword))
                {
                    objAPIRequest.NewPassword = BCrypt.Net.BCrypt.HashPassword(objAPIRequest.NewPassword.Trim());
                }

                NexusDAL objNexusDAL = new NexusDAL(_objAppDb);
                intResult = objNexusDAL.ChangePassword(objAPIRequest!);

                if (intResult <= 0)
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.Failed_To_Update_Password);
                }
            }
            catch (Exception)
            {
                intErrorCode = Convert.ToInt32(ErrorCode.Technical_Error);
            }
            finally
            {
                if (intErrorCode > 0)
                {
                    string? strEnumName = Enum.GetName(typeof(ErrorCode), intErrorCode);
                    string errorMessage;

                    if (!string.IsNullOrEmpty(strEnumName))
                    {
                        errorMessage = strEnumName.Replace('_', ' ');
                    }
                    else
                    {
                        try
                        {
                            NexusDAL objError = new NexusDAL(_objAppDb);
                            errorMessage = (string?)objError.GetErrorMessage(intErrorCode) ?? "An error occurred. Please try again.";
                        }
                        catch
                        {
                            errorMessage = "An unexpected error occurred. Please try again later.";
                        }
                    }

                    objResponse = new Response<object>
                    {
                        ReturnCode = intErrorCode,
                        ReturnMessage = errorMessage
                    };
                }
                else
                {
                    objResponse = new Response<object>
                    {
                        ReturnCode = 0,
                        ReturnMessage = "Password changed successfully",
                        RecordCount = 1,
                        Data = intResult
                    };
                }

                objResponse.ResponseTime = Math.Round((DateTime.Now - dtStartResponseTime).TotalMilliseconds).ToString();
            }

            return objResponse;
        }


        /// <summary>
        /// Retrieves a list of employees filtered by the specified search criteria.
        /// </summary>
        /// <param name="objEmployeeListDTO">Request payload containing search criteria and filter parameters.</param>
        /// <returns>A DataTable populated with matching employee records.</returns>
        public Response<object>? GetEmployees(GetRequest objAPIRequest)
        {

            DateTime dtStartResponseTime = DateTime.Now;
            int intErrorCode = 0;
            string strResponse = string.Empty;

            Response<object>? objResponse = null;
            DataTable? dtEmployees = null;
            NexusDAL? objNexusDAL;

            try
            {
                
                if (intErrorCode == 0)
                {
                    if (!string.IsNullOrWhiteSpace(Convert.ToString(objAPIRequest.SearchWord)))
                    {
                        if (!Regex.IsMatch(Convert.ToString(objAPIRequest.SearchWord!), _objRegularExpressions?.RegExSearchWord ?? string.Empty))
                        {
                            intErrorCode = Convert.ToInt32(ErrorCode.Invalid_SearchWord);
                        }
                    }
                }
                if (intErrorCode == 0)
                {

                    if (objAPIRequest != null)
                    {
                        try
                        {

                            objNexusDAL = new NexusDAL(_objAppDb);
                            dtEmployees = objNexusDAL.GetEmployees(objAPIRequest);

                            if (dtEmployees != null && dtEmployees.Rows.Count > 0)
                            {
                                strResponse = JsonConvert.SerializeObject(dtEmployees, Formatting.Indented);
                            }
                            else
                            {
                                intErrorCode = Convert.ToInt32(ErrorCode.No_Records_Found);
                            }
                        }
                        catch (Exception)
                        {

                            intErrorCode = Convert.ToInt32(ErrorCode.Technical_Error);
                        }
                    }
                }
            }
            catch (Exception)
            {
                intErrorCode = Convert.ToInt32(ErrorCode.Technical_Error);

            }
            finally
            {
                try
                {
                    if (intErrorCode > 0)
                    {
                        objResponse = new Response<object>
                        {
                            ReturnCode = -1,
                            ReturnMessage = Enum.GetName(typeof(ErrorCode), intErrorCode).Replace('_', ' '),
                        };
                    }
                    else if (intErrorCode == 0 && dtEmployees != null && dtEmployees.Rows.Count > 0)
                    {
                        objResponse = new()
                        {
                            ReturnCode = 0,
                            ReturnMessage = "success",
                            RecordCount = dtEmployees.Rows.Count,
                        };

                        objResponse.Data = JsonConvert.DeserializeObject(strResponse);

                    }
                    objResponse?.ResponseTime = Math.Round((DateTime.Now - dtStartResponseTime).TotalMilliseconds).ToString();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    throw;
                }

            }
            return objResponse;
        }

        /// <summary>
        /// Inserts a new employee record into the database.
        /// </summary>
        /// <param name="objAPIRequest">Request payload containing the new employee details.</param>
        /// <returns>A Response object containing the result of the insertion operation, or null if the operation fails.</returns>
        public Response<object> InsertEmployee(InsertRequest objAPIRequest)
        {

            DateTime dtStartResponseTime = DateTime.Now;
            int intErrorCode = 0;

            Response<object>? objResponse = null;
            object? objResult = null;

            try
            {

                if (objAPIRequest == null)
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.Invalid_Request);
                }
                else if (string.IsNullOrWhiteSpace(objAPIRequest.FirstName))
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.FirstName_Required);
                }
                else if (!Regex.IsMatch(objAPIRequest.FirstName, _objRegularExpressions.RegExForName ?? string.Empty))
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.Invalid_FirstName);
                }
                else if (!string.IsNullOrWhiteSpace(objAPIRequest.LastName) &&
                         !Regex.IsMatch(objAPIRequest.LastName, _objRegularExpressions.RegExForName ?? string.Empty))
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.Invalid_LastName);
                }
                else if (string.IsNullOrWhiteSpace(objAPIRequest.Email))
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.Email_Required);
                }
                else if (!Regex.IsMatch(objAPIRequest.Email, _objRegularExpressions.RegExForEmail ?? string.Empty))
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.Invalid_Email);
                }
                else if (string.IsNullOrWhiteSpace(objAPIRequest.MobileNo))
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.Mobile_Number_Required);
                }
                else if (!Regex.IsMatch(objAPIRequest.MobileNo, _objRegularExpressions.RegExForMobileNo ?? string.Empty))
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.Invalid_Mobile_Number);
                }
                else if (string.IsNullOrWhiteSpace(objAPIRequest.Gender))
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.Gender_Required);
                }
                else if (!Regex.IsMatch(objAPIRequest.Gender, _objRegularExpressions.RegExForGender ?? string.Empty))
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.Invalid_Gender);
                }
                else if (string.IsNullOrWhiteSpace(objAPIRequest.EmployeeType))
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.EmployeeType_Required);
                }
                else if (!Regex.IsMatch(objAPIRequest.EmployeeType, _objRegularExpressions.RegExForEmployeeType ?? string.Empty))
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.Invalid_EmployeeType);
                }
                else if (objAPIRequest.DepartmentId <= 0)
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.Department_Required);
                }
                else if (objAPIRequest.DesignationId <= 0)
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.Designation_Required);
                }
                else if (objAPIRequest.CompanyId <= 0)
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.Company_Required);
                }
                else if (objAPIRequest.SiteId <= 0)
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.Site_Required);
                }
                else if (objAPIRequest.DateOfJoining == null || objAPIRequest.DateOfJoining == DateTime.MinValue)
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.DateOfJoining_Required);
                }
                else if (string.IsNullOrWhiteSpace(objAPIRequest.CreatedBy))
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.CreatedBy_Required);
                }
                else if (!Regex.IsMatch(objAPIRequest.CreatedBy, _objRegularExpressions.RegExForName))
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.Invalid_CreatedBy);
                }
                else if (!string.IsNullOrWhiteSpace(objAPIRequest.IPAddress))
                {
                    if (objAPIRequest.IPAddress.Length > 64 ||
                        !System.Net.IPAddress.TryParse(objAPIRequest.IPAddress, out _))
                    {
                        intErrorCode = Convert.ToInt32(ErrorCode.Invalid_IPAddress);
                    }
                }

                if (intErrorCode == 0)
                {
                    try
                    {
                        NexusDAL objNexusDAL = new NexusDAL(_objAppDb);
                        objResult = objNexusDAL.InsertEmployee(objAPIRequest!);

                        if (objResult == null || objResult == DBNull.Value || objResult.ToString() == "ERROR")
                        {
                            intErrorCode = Convert.ToInt32(ErrorCode.No_Records_Found);
                        }
                    }
                    finally
                    {
                        objAPIRequest = null;
                    }
                }
            }
            catch (Exception)
            {
                intErrorCode = Convert.ToInt32(ErrorCode.Technical_Error);
            }
            finally
            {
                if (intErrorCode > 0)
                {
                    string? strEnumName = Enum.GetName(typeof(ErrorCode), intErrorCode);
                    string errorMessage;

                    if (!string.IsNullOrEmpty(strEnumName))
                    {
                        errorMessage = strEnumName.Replace('_', ' ');
                    }
                    else
                    {
                        try
                        {
                            // Fetch error message safely
                            NexusDAL objError = new NexusDAL(_objAppDb);
                            errorMessage = (string?)objError.GetErrorMessage(intErrorCode) ?? "An error occurred. Please try again."; ;
                        }
                        catch
                        {
                            // Fallback string if DB connection fails
                            errorMessage = "An unexpected error occurred. Please try again later.";
                        }
                    }


                    objResponse = new Response<object>
                    {
                        ReturnCode = intErrorCode,
                        ReturnMessage = errorMessage
                    };
                }
                else
                {
                    objResponse = new Response<object>
                    {
                        ReturnCode = 0,
                        ReturnMessage = "Insert successful",
                        RecordCount = 0,
                        Data = objResult
                    };
                }

                objResponse.ResponseTime = Math.Round((DateTime.Now - dtStartResponseTime).TotalMilliseconds).ToString();
            }

            return objResponse;
        }

        /// <summary>
        /// Updates an existing employee record in the database using its unique identifier.
        /// </summary>
        /// <param name="objAPIRequest">Request payload containing updated employee details and the target employee ID.</param>
        /// <returns>A Response object containing the result of the update operation, or null if the operation fails.</returns>
        public Response<object> UpdateEmployeeById(UpdateRequest objAPIRequest)
        {

            DateTime dtStartResponseTime = DateTime.Now;
            int intErrorCode = 0;

            Response<object>? objResponse = null;
            object? objResult = null;

            try
            {

                if (objAPIRequest == null)
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.Invalid_Request);
                }
                else if (string.IsNullOrWhiteSpace(objAPIRequest.Id))
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.Id_Required);
                }
                else if (!Regex.IsMatch(objAPIRequest.Id, _objRegularExpressions.RegExForId ?? string.Empty))
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.Invalid_Id);
                }
                else if (string.IsNullOrWhiteSpace(objAPIRequest.UpdatedBy))
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.UpdatedBy_Required);
                }
                else if (!Regex.IsMatch(objAPIRequest.UpdatedBy, _objRegularExpressions.RegExForName ?? string.Empty))
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.Invalid_UpdatedBy);
                }
                else if (objAPIRequest.FirstName == null &&
                         objAPIRequest.LastName == null &&
                         objAPIRequest.MiddleName == null &&
                         objAPIRequest.Email == null &&
                         objAPIRequest.Cmail == null &&
                         objAPIRequest.MobileNo == null &&
                         objAPIRequest.Gender == null &&
                         objAPIRequest.DepartmentId == null &&
                         objAPIRequest.DesignationId == null &&
                         objAPIRequest.CurrentSalary == null &&
                         objAPIRequest.CompanyId == null &&
                         objAPIRequest.SiteId == null &&
                         objAPIRequest.EmployeeCode == null &&
                         objAPIRequest.EmployeeType == null &&
                         objAPIRequest.ReportingManagerId == null &&
                         objAPIRequest.ExperienceYears == null &&
                         objAPIRequest.IsPublished == null &&
                         objAPIRequest.PublishedBy == null &&
                         objAPIRequest.DisplayOnWeb == null &&
                         objAPIRequest.SortOrder == null &&
                         objAPIRequest.Tag == null &&
                         objAPIRequest.Comments == null &&
                         objAPIRequest.IPAddress == null &&
                         objAPIRequest.DateOfJoining == null)
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.At_Least_One_Field_Required);
                }
                else if (!string.IsNullOrWhiteSpace(objAPIRequest.FirstName) &&
                         !Regex.IsMatch(objAPIRequest.FirstName, _objRegularExpressions.RegExForName ?? string.Empty))
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.Invalid_FirstName);
                }
                else if (!string.IsNullOrWhiteSpace(objAPIRequest.LastName) &&
                         !Regex.IsMatch(objAPIRequest.LastName, _objRegularExpressions.RegExForName ?? string.Empty))
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.Invalid_LastName);
                }
                else if (!string.IsNullOrWhiteSpace(objAPIRequest.Email) &&
                         !Regex.IsMatch(objAPIRequest.Email, _objRegularExpressions.RegExForEmail ?? string.Empty))
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.Invalid_Email);
                }
                else if (!string.IsNullOrWhiteSpace(objAPIRequest.MobileNo) &&
                         !Regex.IsMatch(objAPIRequest.MobileNo, _objRegularExpressions.RegExForMobileNo ?? string.Empty))
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.Invalid_Mobile_Number);
                }
                else if (!string.IsNullOrWhiteSpace(objAPIRequest.Gender) &&
                         !Regex.IsMatch(objAPIRequest.Gender, _objRegularExpressions.RegExForGender ?? string.Empty))
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.Invalid_Gender);
                }
                else if (!string.IsNullOrWhiteSpace(objAPIRequest.EmployeeType) &&
                         !Regex.IsMatch(objAPIRequest.EmployeeType, _objRegularExpressions.RegExForEmployeeType ?? string.Empty))
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.Invalid_EmployeeType);
                }
                else if (objAPIRequest.DepartmentId.HasValue && objAPIRequest.DepartmentId <= 0)
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.Invalid_Department);
                }
                else if (objAPIRequest.DesignationId.HasValue && objAPIRequest.DesignationId <= 0)
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.Invalid_Designation);
                }
                else if (objAPIRequest.CompanyId.HasValue && objAPIRequest.CompanyId <= 0)
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.Invalid_Company);
                }
                else if (objAPIRequest.SiteId.HasValue && objAPIRequest.SiteId <= 0)
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.Invalid_Site);
                }
                else if (!string.IsNullOrWhiteSpace(objAPIRequest.IPAddress))
                {
                    if (objAPIRequest.IPAddress.Length > 64 ||
                        !System.Net.IPAddress.TryParse(objAPIRequest.IPAddress, out _))
                    {
                        intErrorCode = Convert.ToInt32(ErrorCode.Invalid_IPAddress);
                    }
                }

                if (intErrorCode == 0)
                {
                    try
                    {
                        NexusDAL objNexusDAL = new NexusDAL(_objAppDb);
                        objResult = objNexusDAL.UpdateEmployeeById(objAPIRequest!);

                        if (objResult == null || objResult == DBNull.Value || objResult.ToString() == "ERROR")
                        {
                            intErrorCode = Convert.ToInt32(ErrorCode.No_Records_Found);
                        }
                    }
                    finally
                    {
                        objAPIRequest = null;
                    }
                }
            }
            catch (Exception)
            {
                intErrorCode = Convert.ToInt32(ErrorCode.Technical_Error);
            }
            finally
            {
                if (intErrorCode > 0)
                {
                    string? strEnumName = Enum.GetName(typeof(ErrorCode), intErrorCode);
                    string errorMessage;

                    if (!string.IsNullOrEmpty(strEnumName))
                    {
                        errorMessage = strEnumName.Replace('_', ' ');
                    }
                    else
                    {
                        try
                        {
                            // Fetch error message safely
                            NexusDAL objError = new NexusDAL(_objAppDb);
                            errorMessage = (string?)objError.GetErrorMessage(intErrorCode) ?? "An error occurred. Please try again."; ;
                        }
                        catch
                        {
                            // Fallback string if DB connection fails
                            errorMessage = "An unexpected error occurred. Please try again later.";
                        }
                    }


                    objResponse = new Response<object>
                    {
                        ReturnCode = intErrorCode,
                        ReturnMessage = errorMessage
                    };
                }
                else
                {
                    objResponse = new Response<object>
                    {
                        ReturnCode = 0,
                        ReturnMessage = "Update Successful",
                        RecordCount = 0,
                        Data = objResult
                    };
                }

                objResponse.ResponseTime = Math.Round((DateTime.Now - dtStartResponseTime).TotalMilliseconds).ToString();
            }

            return objResponse;
        }


        /// <summary>
        /// Deletes an employee record based on the specified identifier.
        /// </summary>
        /// <param name="objAPIRequest">Request payload containing deletion parameters and target ID.</param>
        /// <returns>A Response object containing execution results, or null.</returns>
        public Response<object> DeleteEmployeeById(DeleteRequest objAPIRequest)
        {

            DateTime dtStartResponseTime = DateTime.Now;
            int intErrorCode = 0;

            Response<object>? objResponse = null;
            int intResult = 0;

            try
            {

                if (objAPIRequest == null)
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.Invalid_Request);
                }
                else if (string.IsNullOrWhiteSpace(objAPIRequest.Id))
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.Id_Required);
                }
                else if (!Regex.IsMatch(objAPIRequest.Id, _objRegularExpressions.RegExForId ?? string.Empty))
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.Invalid_Id);
                }
                else if (string.IsNullOrWhiteSpace(objAPIRequest.DeletedBy))
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.DeletedBy_Required);
                }
                else if (!Regex.IsMatch(objAPIRequest.DeletedBy, _objRegularExpressions.RegExForName ?? string.Empty))
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.Invalid_DeletedBy);
                }
                else if (!string.IsNullOrWhiteSpace(objAPIRequest.IPAddress))
                {
                    // Check both length (for DB safety) and valid IPv4/IPv6 format
                    if (objAPIRequest.IPAddress.Length > 64 ||
                        !System.Net.IPAddress.TryParse(objAPIRequest.IPAddress, out _))
                    {
                        intErrorCode = Convert.ToInt32(ErrorCode.Invalid_IPAddress);
                    }
                }

                if (intErrorCode == 0)
                {

                    try
                    {

                        NexusDAL objNexusDAL = new NexusDAL(_objAppDb);
                        intResult = (int)objNexusDAL.DeleteEmployeeById(objAPIRequest!);

                        if (intResult <= 0)
                        {
                            intErrorCode = Convert.ToInt32(ErrorCode.Record_Already_Deleted);
                        }
                    }

                    finally
                    {
                        objAPIRequest = null;
                    }

                }
            }
            catch (Exception)
            {

                intErrorCode = Convert.ToInt32(ErrorCode.Technical_Error);
            }
            finally
            {

                if (intErrorCode > 0)
                {
                    // First try to resolve the message instantly from memory via Enum
                    string? strEnumName = Enum.GetName(typeof(ErrorCode), intErrorCode);
                    string errorMessage;

                    if (!string.IsNullOrEmpty(strEnumName))
                    {
                        errorMessage = strEnumName.Replace('_', ' ');
                    }
                    else
                    {
                        try
                        {
                            // Fetch error message safely
                            NexusDAL objError = new NexusDAL(_objAppDb);
                            errorMessage = (string?)objError.GetErrorMessage(intErrorCode) ?? "An error occurred. Please try again."; ;
                        }
                        catch
                        {
                            // Fallback string if DB connection fails
                            errorMessage = "An unexpected error occurred. Please try again later.";
                        }
                    }
                    

                    objResponse = new Response<object>
                    {
                        ReturnCode = intErrorCode,
                        ReturnMessage = errorMessage
                    };
                }
                else
                {
                    objResponse = new Response<object>
                    {
                        ReturnCode = 0,
                        ReturnMessage = "Delete Successful",
                        RecordCount = 0,
                        Data = intResult
                    };
                }

                objResponse.ResponseTime = Math.Round((DateTime.Now - dtStartResponseTime).TotalMilliseconds).ToString();
            }

            return objResponse;
        }
    }
}

