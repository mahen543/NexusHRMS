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
        #region Fields & Properties
        private AppDb _objAppDb { get; }
        private readonly RegularExpressions? _objRegularExpressions;
        private readonly General _objGeneral;
        #endregion

        #region Constructors
        public NexusBAL(AppDb objAppDb, General? objGeneral = null, RegularExpressions? objRegularExpressions = null)
        {
            _objAppDb = objAppDb;
            _objGeneral = objGeneral ?? new General();
            _objRegularExpressions = objRegularExpressions;
        }
        #endregion


        #region GetEmployeesBySearch
        //***********************************************************************************************************************
        // Layer                        :   BAL 
        // Method Name                  :   GetEmployees
        // Method Description           :   This method is used to get the Employees details based on Search parameters.
        // Author                       :   Mahendra N
        // Creation Date                :   07 June 2026
        // Input Parameters             :   objEmployeeListDTO
        // Modified Date                : 
        // Modified Reason              :
        // Return Values                :   DataTable
        //-----------------------------------------------------------------------------------------------------------------------
        //  Version             Author                      Date                        Remarks       
        // ----------------------------------------------------------------------------------------------------------------------
        //  1.0                 Mahendra N                  07 June 2026                 Creation
        //***********************************************************************************************************************
        /// <summary>
        /// <c>GetEmployees</c> This method is used to get the Employees details based on Search parameters.
        /// <param>objListDTO</param>
        /// <returns>DataTable</returns> //It returns the Date Table.
        /// </summary> 
        public Response<object>? GetEmployees(GetRequest objAPIRequest)
        {
            #region Common Variables

            DateTime dtStartResponseTime = DateTime.Now;
            int intErrorCode = 0;
            string strResponse = string.Empty;
            #endregion


            #region Specific Variables
            //string searchWord = APIRequest.GetProperty("SearchWord").GetString();
            Response<object>? objResponse = null;
            DataTable? dtEmployees = null;
            NexusDAL? objNexusDAL;
            #endregion


            try
            {
                #region Request data Validations

                #region Search Text Validation

                if (intErrorCode == 0)
                {
                    if (!string.IsNullOrWhiteSpace(Convert.ToString(objAPIRequest.SearchWord)))
                    {
                        if (!Regex.IsMatch(Convert.ToString(objAPIRequest.SearchWord), _objRegularExpressions.RegExSearchWord ?? string.Empty))
                        {
                            intErrorCode = Convert.ToInt32(ErrorCode.Invalid_SearchWord);
                        }
                    }
                }
                #endregion

                #endregion


                #region Calling DAL

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
                #endregion
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
        #endregion

        #region InsertEmployee
        //***************************************************************************************************************************
        // Layer                        :   BAL 
        // Method Name                  :   InsertEmployee
        // Method Description           :   This method is used to insert Employee record into tblEmployee table.
        // Author                       :   Mahendra N
        // Creation Date                :   10 Nov 2023
        // Input Parameters             :   dynamic
        // Modified Date                : 
        // Modified Reason              :
        // Return Values                :   
        //---------------------------------------------------------------------------------------------------------------------------
        //  Version             Author                      Date                        Remarks       
        // --------------------------------------------------------------------------------------------------------------------------
        //  1.0                 Mahendra N                  08 Nov 2026                 Creation
        //***************************************************************************************************************************
        /// <summary>
        /// <c>InsertEmployee</c> This method is used to insert Employee record into tblEmployee table.
        /// <param>dynamic</param>
        /// </summary> 
        public Response<object> InsertEmployee(InsertRequest objAPIRequest)
        {


            #region Common Variables

            DateTime dtStartResponseTime = DateTime.Now;
            int intErrorCode = 0;
            //string strResponse = string.Empty;
            #endregion

            #region Specific Variables

            Response<object>? objResponse = null;
            object? objResult = null;
            //string strId = string.Empty;
            #endregion


            try
            {

                #region Request data Validations

                #region FirstName Validation
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
                #endregion

                #region Optional LastName Validation
                else if (!string.IsNullOrWhiteSpace(objAPIRequest.LastName) &&
                         !Regex.IsMatch(objAPIRequest.LastName, _objRegularExpressions.RegExForName ?? string.Empty))
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.Invalid_LastName);
                }
                #endregion

                #region Email Validation
                else if (string.IsNullOrWhiteSpace(objAPIRequest.Email))
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.Email_Required);
                }
                else if (!Regex.IsMatch(objAPIRequest.Email, _objRegularExpressions.RegExForEmail ?? string.Empty))
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.Invalid_Email);
                }
                #endregion

                #region MobileNumber Validation
                else if (string.IsNullOrWhiteSpace(objAPIRequest.MobileNo))
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.Mobile_Number_Required);
                }
                else if (!Regex.IsMatch(objAPIRequest.MobileNo, _objRegularExpressions.RegExForMobileNo ?? string.Empty))
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.Invalid_Mobile_Number);
                }
                #endregion

                #region Gender Validation
                else if (string.IsNullOrWhiteSpace(objAPIRequest.Gender))
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.Gender_Required);
                }
                else if (!Regex.IsMatch(objAPIRequest.Gender, _objRegularExpressions.RegExForGender ?? string.Empty))
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.Invalid_Gender);
                }
                #endregion

                #region EmployeeType Validation
                else if (string.IsNullOrWhiteSpace(objAPIRequest.EmployeeType))
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.EmployeeType_Required);
                }
                else if (!Regex.IsMatch(objAPIRequest.EmployeeType, _objRegularExpressions.RegExForEmployeeType ?? string.Empty))
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.Invalid_EmployeeType);
                }
                #endregion

                #region Department Validation
                else if (objAPIRequest.DepartmentId <= 0)
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.Department_Required);
                }
                #endregion

                #region Designation Validation
                else if (objAPIRequest.DesignationId <= 0)
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.Designation_Required);
                }
                #endregion

                #region Company Validation
                else if (objAPIRequest.CompanyId <= 0)
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.Company_Required);
                }
                #endregion

                #region Site Validation
                else if (objAPIRequest.SiteId <= 0)
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.Site_Required);
                }
                #endregion

                #region DateOfJoining Validation
                else if (objAPIRequest.DateOfJoining == null || objAPIRequest.DateOfJoining == DateTime.MinValue)
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.DateOfJoining_Required);
                }
                #endregion

                #region CreatedBy Validation
                else if (string.IsNullOrWhiteSpace(objAPIRequest.CreatedBy))
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.CreatedBy_Required);
                }
                else if (!Regex.IsMatch(objAPIRequest.CreatedBy, _objRegularExpressions.RegExForName))
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.Invalid_CreatedBy);
                }
                #endregion

                #region Optional IPAddress Validation
                else if (!string.IsNullOrWhiteSpace(objAPIRequest.IPAddress))
                {
                    if (objAPIRequest.IPAddress.Length > 64 ||
                        !System.Net.IPAddress.TryParse(objAPIRequest.IPAddress, out _))
                    {
                        intErrorCode = Convert.ToInt32(ErrorCode.Invalid_IPAddress);
                    }
                }
                #endregion
                #endregion


                #region Calling DAL

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

                #endregion
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
        #endregion

        #region UpdateEmployeeById

        //****************************************************************************************************
        // Layer                        :   BAL 
        // Method Name                  :   UpdateEmployeeById
        // Method Description           :   This method is used to update Employee record based onId.
        // Author                       :   Mahendra N
        // Creation Date                :   14 Nov 2026
        // Input Parameters             :   dynamic
        // Modified Date                : 
        // Modified Reason              :
        // Return Values                :   
        //----------------------------------------------------------------------------------------------------
        //  Version             Author                      Date                        Remarks       
        // ---------------------------------------------------------------------------------------------------
        //  1.0                 Mahendra N                  04 Nov 2026                 Creation
        //****************************************************************************************************
        /// <summary>
        /// <c>UpdateEmployeeById</c> This method is used to update Employee record based on Id.
        /// <param>dynamic</param>
        /// </summary> 
        public Response<object> UpdateEmployeeById(UpdateRequest objAPIRequest)
        {


            #region Common Variables

            DateTime dtStartResponseTime = DateTime.Now;
            int intErrorCode = 0;
            //string strResponse = string.Empty;
            #endregion

            #region Specific Variables

            Response<object>? objResponse = null;
            // NexusDAL? objNexusDAL = null;
            //string strId = string.Empty;
            object? objResult = null;
            #endregion


            try
            {
                #region Request data Validations
                #region Request data Validations

                if (objAPIRequest == null)
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.Invalid_Request);
                }

                #region Id Validation
                else if (string.IsNullOrWhiteSpace(objAPIRequest.Id))
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.Id_Required);
                }
                else if (!Regex.IsMatch(objAPIRequest.Id, _objRegularExpressions.RegExForId ?? string.Empty))
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.Invalid_Id);
                }
                #endregion

                #region UpdatedBy Validation
                else if (string.IsNullOrWhiteSpace(objAPIRequest.UpdatedBy))
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.UpdatedBy_Required);
                }
                else if (!Regex.IsMatch(objAPIRequest.UpdatedBy, _objRegularExpressions.RegExForName ?? string.Empty))
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.Invalid_UpdatedBy);
                }
                #endregion

                #region Validation: At Least One Field To Update Check
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
                #endregion

                #region Optional Text Validations
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
                #endregion

                #region Optional Foreign Keys / Numeric Checks
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
                #endregion

                #region Optional IPAddress Validation
                else if (!string.IsNullOrWhiteSpace(objAPIRequest.IPAddress))
                {
                    if (objAPIRequest.IPAddress.Length > 64 ||
                        !System.Net.IPAddress.TryParse(objAPIRequest.IPAddress, out _))
                    {
                        intErrorCode = Convert.ToInt32(ErrorCode.Invalid_IPAddress);
                    }
                }
                #endregion
                #endregion
                #region Calling DAL

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

                #endregion
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
        #endregion

        #region DeleteEmployeeById

        //*********************************************************************************************************
        // Layer                        :   BAL 
        // Method Name                  :   DeleteEmployeeById
        // Method Description           :   This method is used to delete the Employee record based on Id.
        // Author                       :   Mahendra N
        // Creation Date                :   07 Nov 2026
        // Input Parameters             :   dynamic
        // Modified Date                : 
        // Modified Reason              :
        // Return Values                :   DataTable
        //---------------------------------------------------------------------------------------------------------
        //  Version             Author                      Date                        Remarks       
        // --------------------------------------------------------------------------------------------------------
        //  1.0                 Mahendra N                  08 Nov 2026                 Creation
        //*********************************************************************************************************
        /// <summary>
        /// <c>DeleteEmployeeById</c> This method is used to delete the Employee record based on Id.
        /// <param>dynamic</param>
        /// </summary> 
        public Response<object> DeleteEmployeeById(DeleteRequest objAPIRequest)
        {

            #region Common Variables

            DateTime dtStartResponseTime = DateTime.Now;
            int intErrorCode = 0;
            #endregion

            #region Specific Variables

            Response<object>? objResponse = null;
            int intResult = 0;
            #endregion


            try
            {

                #region Request data Validations

                if (objAPIRequest == null)
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.Invalid_Request);
                }

                #region Id Validation

                else if (string.IsNullOrWhiteSpace(objAPIRequest.Id))
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.Id_Required);
                }
                else if (!Regex.IsMatch(objAPIRequest.Id, _objRegularExpressions.RegExForId ?? string.Empty))
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.Invalid_Id);
                }
                #endregion

                #region DeletedBy Validation
                else if (string.IsNullOrWhiteSpace(objAPIRequest.DeletedBy))
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.DeletedBy_Required);
                }
                else if (!Regex.IsMatch(objAPIRequest.DeletedBy, _objRegularExpressions.RegExForName ?? string.Empty))
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.Invalid_DeletedBy);
                }
                #endregion

                #region IPAddress Validation
                else if (!string.IsNullOrWhiteSpace(objAPIRequest.IPAddress))
                {
                    // Check both length (for DB safety) and valid IPv4/IPv6 format
                    if (objAPIRequest.IPAddress.Length > 64 ||
                        !System.Net.IPAddress.TryParse(objAPIRequest.IPAddress, out _))
                    {
                        intErrorCode = Convert.ToInt32(ErrorCode.Invalid_IPAddress);
                    }
                }
                #endregion
                #endregion


                #region Calling DAL

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
                #endregion
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
        #endregion

        #endregion
    }
}

