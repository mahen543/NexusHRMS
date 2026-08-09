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
        private AppDb _objAppDb { get; }
        private readonly RegularExpressions? _objRegularExpressions;
        private readonly General _objGeneral;

        public AuthBAL(AppDb objAppDb, General? objGeneral = null, RegularExpressions? objRegularExpressions = null)
        {
            _objAppDb = objAppDb;
            _objGeneral = objGeneral ?? new General();
            _objRegularExpressions = objRegularExpressions;
        }
        
        
        /// <summary>
        /// Validates user login credentials against the stored password hash and returns user authentication details.
        /// </summary>
        /// <param name="objLoginRequest">The login request payload containing credentials.</param>
        /// <returns>A Response object containing the authenticated user's details, or an error status on failure.</returns>
        public Response<UserAuthModel>? AuthenticateUser(LoginRequestDto objLoginRequest)
        {

            DateTime dtStartResponseTime = DateTime.Now;
            int intErrorCode = 0;

            Response<UserAuthModel>? objResponse = null;
            DataTable? dtUser = null;
            AuthDAL? objAuthDAL;
            UserAuthModel? objUserAuth = null;

            try
            {
                if (objLoginRequest == null || string.IsNullOrWhiteSpace(objLoginRequest.UserName) || string.IsNullOrWhiteSpace(objLoginRequest.Password))
                {
                    intErrorCode = Convert.ToInt32(ErrorCode.Invalid_SearchWord); // Or your specific invalid input error code
                }

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

        /// <summary>
        /// Re-evaluates and retrieves an updated user authentication model by user ID.
        /// </summary>
        /// <param name="longUserId">The unique numeric identifier of the target user.</param>
        /// <returns>A Response object containing the refreshed user authentication details.</returns>
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

        /// <summary>
        /// Evaluates user hierarchy level and designation IDs in memory to determine authorization access.
        /// </summary>
        /// <param name="objUserHierarchy">User context built from JWT claims.</param>
        /// <param name="maxAllowedHierarchyLevel">Maximum allowed rank level (where 1 represents the highest authority position).</param>
        /// <param name="allowedDesignationIds">Optional target designation ID filter array.</param>
        /// <returns>True if authorization criteria are satisfied; otherwise, false.</returns>
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
    }
}