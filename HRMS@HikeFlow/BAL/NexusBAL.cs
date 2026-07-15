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
        public NexusBAL(AppDb objAppDb, General objGeneral, RegularExpressions objRegularExpressions = null)
        {
            _objAppDb = objAppDb;
            _objGeneral = objGeneral;
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
        public Response<object>? GetEmployees(Request objAPIRequest)
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
                        if (!Regex.IsMatch(Convert.ToString(objAPIRequest.SearchWord), _objRegularExpressions.RegExSearchWord))
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

                    if (objAPIRequest != null )
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
        public Response<object> InsertEmployee(dynamic objAPIRequest)
        {


            #region Common Variables

            DateTime dtStartResponseTime = DateTime.Now;
            int intErrorCode = 0;
            string strResponse = string.Empty;
            #endregion

            #region Specific Variables

            Response<object>? objResponse = null;
            NexusDAL? objNexusDAL = null;
            string strId = string.Empty;
            #endregion


            try
            {

                #region Default values

                objAPIRequest.IpAddress = _objGeneral.GetIP4Address();
                #endregion

                #region Request data Validations

                #region FirstName Validation
                if (intErrorCode == 0)
                {
                    if (string.IsNullOrWhiteSpace(Convert.ToString(objAPIRequest.FirstName)))
                    {
                        intErrorCode = Convert.ToInt32(ErrorCode.FirstName_Required);
                    }
                    else if (!Regex.IsMatch(Convert.ToString(objAPIRequest.FirstName), _objRegularExpressions?.RegExForName))
                    {
                        intErrorCode = Convert.ToInt32(ErrorCode.Invalid_FirstName);
                    }
                }
                #endregion

                #region LastName Validation
                if (intErrorCode == 0)
                {
                    if (string.IsNullOrWhiteSpace(Convert.ToString(objAPIRequest.LastName)))
                    {
                        intErrorCode = Convert.ToInt32(ErrorCode.LastName_Required);
                    }
                    else if (!Regex.IsMatch(Convert.ToString(objAPIRequest.LastName), _objRegularExpressions?.RegExForName))
                    {
                        intErrorCode = Convert.ToInt32(ErrorCode.Invalid_LastName);
                    }
                }
                #endregion

                #region Email Validation
                if (intErrorCode == 0)
                {
                    if (string.IsNullOrWhiteSpace(Convert.ToString(objAPIRequest.Email)))
                    {
                        intErrorCode = Convert.ToInt32(ErrorCode.Email_Required);
                    }
                    else if (!Regex.IsMatch(Convert.ToString(objAPIRequest.Email), _objRegularExpressions.RegExForEmail))
                    {
                        intErrorCode = Convert.ToInt32(ErrorCode.Invalid_Email);
                    }
                }
                #endregion

                #region MobileNumber Validation
                if (intErrorCode == 0)
                {
                    if (string.IsNullOrWhiteSpace(Convert.ToString(objAPIRequest.MobileNumber)))
                    {
                        intErrorCode = Convert.ToInt32(ErrorCode.Mobile_Number_Required);
                    }
                    else if (!Regex.IsMatch(Convert.ToString(objAPIRequest.MobileNumber), _objRegularExpressions.RegExForMobileNumber))
                    {
                        intErrorCode = Convert.ToInt32(ErrorCode.Invalid_Mobile_Number);
                    }
                }
                #endregion

                #region Department Validation
                if (intErrorCode == 0)
                {
                    if (Convert.ToInt64(objAPIRequest.DepartmentId) <= 0)
                    {
                        intErrorCode = Convert.ToInt32(ErrorCode.Department_Required);
                    }
                }
                #endregion

                #region Degignation Validation
                if (intErrorCode == 0)
                {
                    if (Convert.ToInt64(objAPIRequest.DesignationId) <= 0)
                    {
                        intErrorCode = Convert.ToInt32(ErrorCode.Designation_Required);
                    }
                }
                #endregion

                #region Company Validation
                if (intErrorCode == 0)
                {
                    if (Convert.ToInt64(objAPIRequest.CompanyId) <= 0)
                    {
                        intErrorCode = Convert.ToInt32(ErrorCode.Company_Required);
                    }
                }
                #endregion

                #region Site Validation
                if (intErrorCode == 0)
                {
                    if (Convert.ToInt64(objAPIRequest.SiteId) <= 0)
                    {
                        intErrorCode = Convert.ToInt32(ErrorCode.Site_Required);
                    }
                }
                #endregion

                #region DateOfJoining Validation
                if (intErrorCode == 0)
                {
                    if (string.IsNullOrWhiteSpace(Convert.ToString(objAPIRequest.DateOfJoining)))
                    {
                        intErrorCode = Convert.ToInt32(ErrorCode.DateOfJoining_Required);
                    }
                }
                #endregion

                #region CreatedBy Validation

                if (intErrorCode == 0)
                {
                    if (string.IsNullOrWhiteSpace(Convert.ToString(objAPIRequest.CreatedBy)))
                    {
                        intErrorCode = Convert.ToInt32(ErrorCode.CreatedBy_Required);
                    }
                    else if (!Regex.IsMatch(Convert.ToString(objAPIRequest.CreatedBy), _objRegularExpressions?.RegExForName))
                    {
                        intErrorCode = Convert.ToInt32(ErrorCode.Invalid_CreatedBy);
                    }
                }
                #endregion
                #endregion


                #region Calling DAL

                if (intErrorCode == 0)
                {
                    try
                    {

                        objNexusDAL = new NexusDAL(_objAppDb);
                        strId = (string)objNexusDAL.InsertEmployee(objAPIRequest);


                        if (strId != "ERROR")
                        {
                            strResponse = JsonConvert.SerializeObject(strId, Formatting.Indented);
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
                        NexusDAL objError = new NexusDAL(_objAppDb);
                        objResponse = new Response<object>
                        {
                            ReturnCode = intErrorCode,
                            ReturnMessage = (string)objError.GetErrorMessage(intErrorCode),
                        };
                    }
                    else if (intErrorCode == 0 && strId != null)
                    {
                        objResponse = new Response<object>
                        {
                            ReturnCode = 0,
                            ReturnMessage = "Insert successful",
                            RecordCount = 0
                        };
                        var json = JsonConvert.DeserializeObject<dynamic>(strResponse);

                        objResponse.Data = json;
                    }
                    objResponse.ResponseTime = Math.Round((DateTime.Now - dtStartResponseTime).TotalMilliseconds).ToString();
                }
                catch (Exception)
                {
                    throw;
                }

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
        public Response<object> UpdateEmployeeById(dynamic objAPIRequest)
        {


            #region Common Variables

            DateTime dtStartResponseTime = DateTime.Now;
            int intErrorCode = 0;
            string strResponse = string.Empty;
            #endregion

            #region Specific Variables

            Response<object>? objResponse = null;
            NexusDAL? objNexusDAL = null;
            string strId = string.Empty;
            #endregion


            try
            {
                #region Request data Validations


                #region Id Validation

                if (intErrorCode == 0)
                {
                    if (string.IsNullOrWhiteSpace(Convert.ToString(objAPIRequest.Id)))
                    {
                        intErrorCode = Convert.ToInt32(ErrorCode.Id_Required);
                    }
                    else if (!Regex.IsMatch(Convert.ToString(objAPIRequest.Id), _objRegularExpressions?.RegExForId))
                    {
                        intErrorCode = Convert.ToInt32(ErrorCode.Invalid_Id);
                    }
                }
                #endregion

                #region UpdatedBy Validation

                if (intErrorCode == 0)
                {
                    if (string.IsNullOrWhiteSpace(Convert.ToString(objAPIRequest.UpdatedBy)))
                    {
                        intErrorCode = Convert.ToInt32(ErrorCode.UpdatedBy_Required);
                    }
                    else if (!Regex.IsMatch(Convert.ToString(objAPIRequest.UpdatedBy), _objRegularExpressions?.RegExForName))
                    {
                        intErrorCode = Convert.ToInt32(ErrorCode.Invalid_UpdatedBy);
                    }
                }
                #endregion

                // Any of the below fields are required
                // Required all fields validations at one place
                #endregion


                #region Calling DAL

                if (intErrorCode == 0)
                {
                    if (objAPIRequest != null)
                    {
                        try
                        {

                            objNexusDAL = new NexusDAL(_objAppDb);
                            strId = (string)objNexusDAL.UpdateEmployeeById(objAPIRequest);


                            if (strId != null && strId != "ERROR")
                            {
                                strResponse = JsonConvert.SerializeObject(strId, Formatting.Indented);
                            }
                            else if (strId == "ERROR")
                            {
                                intErrorCode = Convert.ToInt32(ErrorCode.No_Records_Found);
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
            catch (Exception ex)
            {
                intErrorCode = Convert.ToInt32(ErrorCode.Technical_Error);
            }
            finally
            {
                try
                {
                    if (intErrorCode > 0)
                    {
                        NexusDAL objError = new NexusDAL(_objAppDb);
                        objResponse = new Response<object>
                        {
                            ReturnCode = intErrorCode,
                            ReturnMessage = (string)objError.GetErrorMessage(intErrorCode),
                        };

                    }
                    else if (intErrorCode == 0 && strId != null)
                    {
                        objResponse = new Response<object>
                        {
                            ReturnCode = 0,
                            ReturnMessage = "update successful",
                            RecordCount = 0,
                        };
                        var json = JsonConvert.DeserializeObject<dynamic>(strResponse);
                        objResponse.Data = json;

                    }
                    objResponse?.ResponseTime = Math.Round((DateTime.Now - dtStartResponseTime).TotalMilliseconds).ToString();
                }
                catch (Exception)
                {
                }
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
        public Response<object> DeleteEmployeeById(Request objAPIRequest)
        {

            #region Common Variables

            DateTime dtStartResponseTime = DateTime.Now;
            int intErrorCode = 0;
            string strResponse = string.Empty;
            #endregion

            #region Specific Variables

            Response<object>? objResponse = null;
            NexusDAL? objNexusDAL = null;
            int intResult = 0;
            #endregion


            try
            {

                #region Request data Validations

                #region Id Validation

                if (intErrorCode == 0)
                {
                    if (string.IsNullOrWhiteSpace(Convert.ToString(objAPIRequest.Id)))
                    {
                        intErrorCode = Convert.ToInt32(ErrorCode.Id_Required);
                    }
                    else if (objAPIRequest.Id != null && !Regex.IsMatch(Convert.ToString(objAPIRequest.Id), _objRegularExpressions?.RegExForId))
                    {
                        intErrorCode = Convert.ToInt32(ErrorCode.Invalid_Id);
                    }
                }
                #endregion

                #region DeletedBy Validation
                if (intErrorCode == 0)
                {
                    if (string.IsNullOrWhiteSpace(Convert.ToString(objAPIRequest.DeletedBy)))
                    {
                        intErrorCode = Convert.ToInt32(ErrorCode.DeletedBy_Required);
                    }
                    else if (objAPIRequest?.DeletedBy != null && !Regex.IsMatch(Convert.ToString(objAPIRequest.DeletedBy), _objRegularExpressions.RegExForName))
                    {
                        intErrorCode = Convert.ToInt32(ErrorCode.Invalid_DeletedBy);
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
                            intResult = (int)objNexusDAL.DeleteEmployeeById(objAPIRequest);


                            if (intResult > 0)
                            {
                                strResponse = JsonConvert.SerializeObject(intResult, Formatting.Indented);
                            }
                            else
                            {
                                intErrorCode = Convert.ToInt32(ErrorCode.Record_Already_Deleted);
                            }
                        }
                        catch (Exception)
                        {

                            intErrorCode = Convert.ToInt32(ErrorCode.Technical_Error);
                        }
                        finally
                        {
                            objAPIRequest = null;
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
                        NexusDAL objError = new NexusDAL(_objAppDb);
                        objResponse = new Response<object>
                        {
                            ReturnCode = intErrorCode,
                            ReturnMessage = (string)objError.GetErrorMessage(intErrorCode),
                        };
                    }
                    else if (intErrorCode == 0 && intResult != 0)
                    {
                        objResponse = new Response<object>
                        {
                            ReturnCode = 0,
                            ReturnMessage = "Delete Successsful",
                            RecordCount = 0,
                        };

                        objResponse.Data = JsonConvert.DeserializeObject(strResponse);

                    }

                    objResponse.ResponseTime = Math.Round((DateTime.Now - dtStartResponseTime).TotalMilliseconds).ToString();
                }
                catch (Exception)
                {
                    throw;
                }

            }

            return objResponse;
        }

        #endregion
    }
}
