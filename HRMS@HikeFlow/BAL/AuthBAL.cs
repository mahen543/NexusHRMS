using HRMS_Common;
using HRMS_Common.CommonProperties;
using HRMS_HikeFlow.DAL;
using HRMS_HikeFlow.NexusProperties;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using static HRMS_Common.General;
using BCrypt.Net;

namespace HRMS_HikeFlow.NexusBAL
{
    public class AuthBAL
    {
        #region Fields & Properties
        private AppDb _objAppDb { get; }
        private readonly RegularExpressions? _objRegularExpressions;
        private readonly General _objGeneral;
        #endregion

        #region Constructors
        public AuthBAL(AppDb objAppDb, General? objGeneral = null, RegularExpressions? objRegularExpressions = null)
        {
            _objAppDb = objAppDb;
            _objGeneral = objGeneral ?? new General();
            _objRegularExpressions = objRegularExpressions;
        }
        #endregion

        #region AuthenticateUser
        //*************************************************************************************************
        // Layer                        :   BAL 
        // Method Name                  :   AuthenticateUser
        // Method Description           :   Validates user credentials against stored BCrypt password hash 
        //                                  and returns user authentication details for JWT token generation.
        // Author                       :   Mahendra N
        // Creation Date                :   27 July 2026
        // Input Parameters             :   objLoginRequest
        // Return Values                :   Response<UserAuthModel>
        //-------------------------------------------------------------------------------------------------
        //  Version             Author                      Date                        Remarks       
        // ------------------------------------------------------------------------------------------------
        //  1.0                 Mahendra N                  27 July 2026                Creation
        //*************************************************************************************************
        /// <summary>
        /// <c>AuthenticateUser</c> Validates user login credentials and returns user details.
        /// </summary>
        /// <param name="objLoginRequest">The login request transfer object.</param>
        /// <returns>Response&lt;UserAuthModel&gt;</returns>

        public Response<UserAuthModel>? AuthenticateUser(LoginRequestDto objLoginRequest)
        {
            #region Common Variables
            DateTime dtStartResponseTime = DateTime.Now;
            int intErrorCode = 0;
            #endregion

            #region Specific Variables
            Response<UserAuthModel>? objResponse = null;
            DataTable? dtUser = null;
            AuthDAL? objAuthDAL;
            UserAuthModel? objUserAuth = null;
            #endregion

            try
            {
                #region Request Data Validations
                if (objLoginRequest == null || string.IsNullOrWhiteSpace(objLoginRequest.UserName) || string.IsNullOrWhiteSpace(objLoginRequest.Password))
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.Invalid_SearchWord); // Or your specific invalid input error code
                }
                #endregion

                #region Calling DAL & Processing Login Logic
                if (intErrorCode == 0 && objLoginRequest != null)
                {
                    try
                    {
                        objAuthDAL = new AuthDAL(_objAppDb);
                        dtUser = objAuthDAL.GetUserForAuth(objLoginRequest.UserName!);

                        if (dtUser != null && dtUser.Rows.Count > 0)
                        {
                            DataRow dr = dtUser.Rows[0];
                            string storedHash = Convert.ToString(dr["PasswordHash"]) ?? string.Empty;

                            // BCrypt Password Verification
                            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(objLoginRequest.Password.Trim(), storedHash.Trim());

                            if (isPasswordValid)
                            {
                                var guidOrdinal = dtUser.Columns.Contains("EmployeeGuid") ? dtUser.Columns["EmployeeGuid"]!.Ordinal : -1;

                                objUserAuth = new UserAuthModel
                                {
                                    UserId = Convert.ToInt64(dr["UserId"]),
                                    Username = Convert.ToString(dr["Username"]) ?? string.Empty,
                                    PasswordHash = storedHash,
                                    EmployeeGuid = guidOrdinal != -1 ? Convert.ToString(dr["EmployeeGuid"]) ?? string.Empty : string.Empty,
                                    DesignationId = Convert.ToInt64(dr["DesignationId"]),
                                    HierarchyLevel = Convert.ToByte(dr["HierarchyLevel"])
                                };
                            }
                            else
                            {
                                intErrorCode = Convert.ToInt32(ErrorCode.No_Records_Found); // Invalid credentials
                            }
                        }
                        else
                        {
                            intErrorCode = Convert.ToInt32(ErrorCode.No_Records_Found); // User not found
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
                        objResponse = new Response<UserAuthModel>
                        {
                            ReturnCode = -1,
                            ReturnMessage = Enum.GetName(typeof(ErrorCode), intErrorCode)?.Replace('_', ' ') ?? "Error",
                        };
                    }
                    else if (intErrorCode == 0 && objUserAuth != null)
                    {
                        objResponse = new Response<UserAuthModel>
                        {
                            ReturnCode = 0,
                            ReturnMessage = "success",
                            RecordCount = 1,
                            Data = objUserAuth
                        };
                    }

                    if (objResponse != null)
                    {
                        objResponse.ResponseTime = Math.Round((DateTime.Now - dtStartResponseTime).TotalMilliseconds).ToString();
                    }
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

        #region RefreshTokenDetails
        //*************************************************************************************************
        // Layer                        :   BAL 
        // Method Name                  :   RefreshTokenDetails
        // Method Description           :   Re-evaluates and returns updated user authentication model by UserId.
        // Author                       :   Mahendra N
        // Creation Date                :   27 July 2026
        // Input Parameters             :   longUserId
        // Return Values                :   Response<UserAuthModel>
        //*************************************************************************************************
        public Response<UserAuthModel>? RefreshTokenDetails(long longUserId)
        {
            DateTime dtStartResponseTime = DateTime.Now;
            int intErrorCode = 0;
            Response<UserAuthModel>? objResponse = null;
            UserAuthModel? objUserAuth = null;

            try
            {
                if (longUserId <= 0)
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.Invalid_SearchWord);
                }

                if (intErrorCode == 0)
                {
                    AuthDAL objAuthDAL = new AuthDAL(_objAppDb);
                    DataTable dtUser = objAuthDAL.GetUserByIdForRefresh(longUserId);

                    if (dtUser != null && dtUser.Rows.Count > 0)
                    {
                        DataRow dr = dtUser.Rows[0];
                        var guidOrdinal = dtUser.Columns.Contains("EmployeeGuid") ? dtUser.Columns["EmployeeGuid"].Ordinal : -1;

                        objUserAuth = new UserAuthModel
                        {
                            UserId = Convert.ToInt64(dr["UserId"]),
                            Username = Convert.ToString(dr["Username"]) ?? string.Empty,
                            PasswordHash = Convert.ToString(dr["PasswordHash"]) ?? string.Empty,
                            EmployeeGuid = guidOrdinal != -1 ? Convert.ToString(dr["EmployeeGuid"]) ?? string.Empty : string.Empty,
                            DesignationId = Convert.ToInt64(dr["DesignationId"]),
                            HierarchyLevel = Convert.ToByte(dr["HierarchyLevel"])
                        };
                    }
                    else
                    {
                        intErrorCode = Convert.ToInt32(ErrorCode.No_Records_Found);
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
                    objResponse = new Response<UserAuthModel>
                    {
                        ReturnCode = -1,
                        ReturnMessage = Enum.GetName(typeof(ErrorCode), intErrorCode)?.Replace('_', ' ') ?? "Error",
                    };
                }
                else if (intErrorCode == 0 && objUserAuth != null)
                {
                    objResponse = new Response<UserAuthModel>
                    {
                        ReturnCode = 0,
                        ReturnMessage = "success",
                        RecordCount = 1,
                        Data = objUserAuth
                    };
                }

                if (objResponse != null)
                {
                    objResponse.ResponseTime = Math.Round((DateTime.Now - dtStartResponseTime).TotalMilliseconds).ToString();
                }
            }

            return objResponse;
        }
        #endregion

        #region ValidateHierarchyAuthorization
        //*************************************************************************************************
        // Layer                        :   BAL 
        // Method Name                  :   ValidateHierarchyAuthorization
        // Method Description           :   Core in-memory RBAC rule evaluation evaluating user hierarchy rank 
        //                                  and designation IDs without hitting the database.
        // Author                       :   Mahendra N
        // Creation Date                :   27 July 2026
        // Input Parameters             :   objUserHierarchy, maxAllowedHierarchyLevel, allowedDesignationIds
        // Return Values                :   bool
        //-------------------------------------------------------------------------------------------------
        //  Version             Author                      Date                        Remarks       
        // ------------------------------------------------------------------------------------------------
        //  1.0                 Mahendra N                  27 July 2026                Creation
        //*************************************************************************************************
        /// <summary>
        /// <c>ValidateHierarchyAuthorization</c> Validates whether a user hierarchy context satisfies policy rules.
        /// </summary>
        /// <param name="objUserHierarchy">User context built from JWT claims.</param>
        /// <param name="maxAllowedHierarchyLevel">Maximum allowed rank (1 is highest).</param>
        /// <param name="allowedDesignationIds">Optional target Designation ID filter array.</param>
        /// <returns>bool</returns>
        public bool ValidateHierarchyAuthorization(UserHierarchyModel objUserHierarchy, byte maxAllowedHierarchyLevel, long[] allowedDesignationIds)
        {
            if (objUserHierarchy == null)
            {
                return false;
            }

            // Rule 1: Hierarchy Level 1 (Top executive position) possesses global override authority
            if (objUserHierarchy.HierarchyLevel == 1)
            {
                return true;
            }

            // Rule 2: Rank Check (Lower byte value = higher authority position)
            if (objUserHierarchy.HierarchyLevel > maxAllowedHierarchyLevel)
            {
                return false;
            }

            // Rule 3: Designation Check (Passes if no specific designations specified, or if user's DesignationId matches)
            if (allowedDesignationIds == null || allowedDesignationIds.Length == 0)
            {
                return true;
            }

            return allowedDesignationIds.Contains(objUserHierarchy.DesignationId);
        }
        #endregion
    }
}