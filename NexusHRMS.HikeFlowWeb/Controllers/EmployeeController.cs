using HRMS_Common;
using HRMS_Common.CommonProperties;
using HRMS_HikeFlow.NexusBAL;
using HRMS_HikeFlow.NexusProperties;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NexusHRMS.HikeFlowWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly AppDb _Db;
        private readonly General _objGeneral;
        private readonly RegularExpressions? _objRegularExpressions;

        public EmployeeController(AppDb db, General objGeneral, IOptions<RegularExpressions>? objRegularExpressions)
        {
            _Db = db;
            _objGeneral = objGeneral;
            _objRegularExpressions = objRegularExpressions?.Value;
        }


        [HttpPost("getEmployee")]
        //[Authorize] // Requires any valid JWT token
       [Authorize(Policy = "HRDirectorship")]
        public async Task<ContentResult?> GetEmployee(GetRequest objAPIRequest)
        {
            NexusBAL? objNexusBAL = null;
            ContentResult? objContentResult = null;
            Response<object>? objAPIResponse = null;
            object? objResult = null;

            Response<object> objResponse = new()
            {
                ReturnCode = Convert.ToInt32((int)General.CommonResponseErrorCodes.InvalidRequest),
                ReturnMessage = nameof(General.CommonResponseErrorCodes.InvalidRequest)
            };
            int intStatusCode = 0;


            #region After validating request

            try
            {
                if (_Db != null)
                {
                    Task<Response<object>?> tskResponse = Task.Run(() =>
                    {
                        objNexusBAL = new NexusBAL(_Db, _objGeneral, _objRegularExpressions);
                        objAPIResponse = objNexusBAL.GetEmployees(objAPIRequest);
                        return objAPIResponse;
                    });
                    objAPIResponse = await tskResponse;

                    if (objAPIResponse != null)
                    {
                        objResult = objAPIResponse;
                    }
                    else
                    {
                        objResult = objResponse;
                    }
                }
                else
                {
                    objResult = objResponse;
                }
                intStatusCode = (int)General.CommonResponseErrorCodes.Success;
            }

            catch (Exception)
            {
                intStatusCode = (int)General.CommonResponseErrorCodes.InvalidRequest;
            }
            #endregion

            #region output converting xml or json

            objContentResult = new ContentResult()
            {
                Content = JsonConvert.SerializeObject(objResult),
                ContentType = "application/json",
                StatusCode = intStatusCode
            };
            
            return objContentResult;
            #endregion
        }

        #region InsertQuarterlyRelease
        //*******************************************************************************************************************************
        // Layer                        :   Controller 
        // Method Name                  :   InsertEmployee
        // Method Description           :   This method is used to insert Employee record into the tblEmployee table.
        // Author                       :   Mahendra N
        // Creation Date                :   10 Nov 2026
        // Input Parameters             :   dynamic
        // Modified Date                : 
        // Modified Reason              :
        // Return Values                :   
        //-------------------------------------------------------------------------------------------------------------------------------
        //  Version             Author                      Date                        Remarks       
        // ------------------------------------------------------------------------------------------------------------------------------
        //  1.0                 Mahendra N                  10 Nov 2026                 Creation
        //*******************************************************************************************************************************
        /// <summary>
        /// <param>objAPIRequest</param>
        /// </summary>
        /// 
        //[Authorize(Roles = "Admin")]
        //[Authorize]
        [HttpPost("insertEmployee")]
        public async Task<ContentResult?> InsertEmployee(InsertRequest objAPIRequest)
        {


            #region Variable Declaration
            Response<object>? objAPIResponse = null;
            //var HeaderType = Request.ContentType;
            NexusBAL? objNexusBAL = null;
            ContentResult? objContentResult = null;
            object? objResult = null;
            int intStatusCode = 0;
            #endregion


            Response<object>? objResponse = new()
            {
                ReturnCode = Convert.ToInt32((int)General.CommonResponseErrorCodes.InvalidRequest),
                ReturnMessage = Enum.GetName(typeof(General.CommonResponseErrorCodes), General.CommonResponseErrorCodes.InvalidRequest)
            };


            #region After validating request

            try
            {
                if (objAPIRequest != null)
                {
                    // Attach IP address cleanly before passing to BAL
                    string clientIp = _objGeneral.GetIP4Address();
                    objAPIRequest = objAPIRequest with { IPAddress = clientIp };

                    objAPIResponse = await Task.Run(() =>
                    {

                        objNexusBAL = new NexusBAL(_Db, _objGeneral, _objRegularExpressions);
                        return objNexusBAL.InsertEmployee(objAPIRequest);
                    });
                }
                if (objAPIResponse != null)
                {
                    objResult = objAPIResponse;
                }
                else
                {
                    objResult = objResponse;
                    intStatusCode = (int)General.CommonResponseErrorCodes.InvalidRequest;
                }
            }
            catch (Exception)
            {
                intStatusCode = (int)General.CommonResponseErrorCodes.InvalidRequest;
                objResult = objResponse;
            }
            #endregion

            #region output converting xml or json

            objContentResult = new ContentResult() { Content = JsonConvert.SerializeObject(objResult), ContentType = "application/json", StatusCode = intStatusCode };
            return objContentResult;
            #endregion
        }
        #endregion


        [HttpPost("updateEmployee")]
        public async Task<ContentResult?> UpdateEmployeeById(UpdateRequest objAPIRequest)
        {

            var HeaderType = Request.ContentType;
            NexusBAL? objNexusBAL = null;
            ContentResult? objContentResult = null;
            Response<object>? UpdateEmployeeById = null;
            object? objResult = null;

            // Attach IP address cleanly before passing to BAL
            string clientIp = _objGeneral.GetIP4Address();
            objAPIRequest = objAPIRequest with { IPAddress = clientIp };

            Response<object>? objResponse = new()
            {
                ReturnCode = Convert.ToInt32(((int)General.CommonResponseErrorCodes.InvalidRequest)),
                ReturnMessage = Enum.GetName(typeof(General.CommonResponseErrorCodes), General.CommonResponseErrorCodes.InvalidRequest)
            };
            int intStatusCode = 0;

            #region After validating request

            try
            {
                if (_Db != null)
                {
                    Task<Response<object>> tskResponse = Task.Run(() =>
                    {

                        objNexusBAL = new NexusBAL(_Db, _objGeneral, _objRegularExpressions);
                        UpdateEmployeeById = objNexusBAL.UpdateEmployeeById(objAPIRequest);

                        return UpdateEmployeeById;
                    });
                    UpdateEmployeeById = await tskResponse;

                    if (UpdateEmployeeById != null)
                    {
                        objResult = UpdateEmployeeById;
                    }
                    else
                    {
                        objResult = objResponse;
                    }
                }
                else
                {
                    objResult = objResponse;
                }
                intStatusCode = (int)General.CommonResponseErrorCodes.Success;
            }
            catch (Exception)
            {
                intStatusCode = (int)General.CommonResponseErrorCodes.InvalidRequest;
            }
            #endregion

            #region output converting xml or json

            objContentResult = new ContentResult() { Content = JsonConvert.SerializeObject(objResult), ContentType = "application/json", StatusCode = intStatusCode };

            return objContentResult;
            #endregion
        }

        #region DeleteEmployeeById
        //*******************************************************************************************************
        // Layer                        :   Controller 
        // Method Name                  :   DeleteEmployeeById
        // Method Description           :   This method is used to delete the Employee record based on Id.
        // Author                       :   Mahendra N
        // Creation Date                :   08 Nov 2026
        // Input Parameters             :   objAPIRequest
        // Modified Date                : 
        // Modified Reason              :
        // Return Values                :   
        //-------------------------------------------------------------------------------------------------------
        //  Version             Author                      Date                        Remarks       
        // ------------------------------------------------------------------------------------------------------
        //  1.0                 Mahendra N                  08 Nov 2026                 Creation
        //*******************************************************************************************************
        /// <summary>
        /// <param>objAPIRequest</param>
        /// </summary>
        /// 
        [HttpPost("deleteEmployee")]
        public async Task<ContentResult?> DeleteEmployeeById(DeleteRequest objAPIRequest)
        {

            NexusBAL? objNexusBAL = null;
            ContentResult? objContentResult = null;
            Response<object>? objAPIResponse = null;
            object? objResult = null;

            string clientIp = _objGeneral.GetIP4Address();
            objAPIRequest = objAPIRequest with { IPAddress = clientIp };

            Response<object>? objResponse = new()
            {
                ReturnCode = Convert.ToInt32((int)General.CommonResponseErrorCodes.InvalidRequest),
                ReturnMessage = Enum.GetName(typeof(General.CommonResponseErrorCodes), General.CommonResponseErrorCodes.InvalidRequest)
            };

            int intStatusCode = 0;


            #region After validating request

            try
            {
                if (_Db != null)
                {
                    Task<Response<object>> tskResponse = Task.Run(() =>
                    {

                        objNexusBAL = new NexusBAL(_Db, _objGeneral, _objRegularExpressions);
                        objAPIResponse = objNexusBAL.DeleteEmployeeById(objAPIRequest);


                        return objAPIResponse;
                    });
                    objAPIResponse = await tskResponse;

                    if (objAPIResponse != null)
                    {
                        objResult = objAPIResponse;
                    }
                    else
                    {
                        objResult = objResponse;
                    }
                }
                else
                {
                    objResult = objResponse;
                }
                intStatusCode = (int)General.CommonResponseErrorCodes.Success;
            }
            catch (Exception)
            {
                intStatusCode = (int)General.CommonResponseErrorCodes.InvalidRequest;
            }
            #endregion

            #region output converting xml or json

            objContentResult = new ContentResult() { 
                Content = JsonConvert.SerializeObject(objResult),
                ContentType = "application/json",
                StatusCode = intStatusCode 
            };


            return objContentResult;
            #endregion
        }
        #endregion

    }

}
