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

        /// <summary>
        /// Updates an employee's password.
        /// </summary>
        /// <param name="request">Password change details.</param>
        /// <returns>JSON result with status.</returns>
        // CHANGED: Changed from [HttpPost("changePassword")] to [HttpPatch("changePassword")]
        [HttpPatch("changePassword")]
        public async Task<ContentResult?> ChangePassword([FromBody] ChangePasswordRequestDto request)
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

            try
            {
                if (_Db != null)
                {
                    Task<Response<object>?> tskResponse = Task.Run(() =>
                    {
                        objNexusBAL = new NexusBAL(_Db, _objGeneral, _objRegularExpressions);
                        objAPIResponse = objNexusBAL?.ChangePassword(request);
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

            objContentResult = new ContentResult()
            {
                Content = JsonConvert.SerializeObject(objResult),
                ContentType = "application/json",
                StatusCode = intStatusCode
            };

            return objContentResult;
        }

        /// <summary>
        /// Retrieves employee details matching search parameters.
        /// </summary>
        /// <param name="objAPIRequest">Filter criteria.</param>
        /// <returns>JSON list of employees.</returns>
        /// 
        // CHANGED: Changed from [HttpPost("getEmployee")] to [HttpGet("getEmployee")]
        // Added [FromQuery] so query parameters bind correctly in GET requests
        [HttpGet("getEmployee")]
        [Authorize(Policy = "GeneralEmployee")]
        public async Task<ContentResult?> GetEmployee([FromQuery] GetRequest objAPIRequest)
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

            objContentResult = new ContentResult()
            {
                Content = JsonConvert.SerializeObject(objResult),
                ContentType = "application/json",
                StatusCode = intStatusCode
            };
            
            return objContentResult;
        }

        /// <summary>
        /// Inserts a new employee record into the database along with capturing the requester's IP address.
        /// </summary>
        /// <param name="objAPIRequest">The employee data request payload to insert.</param>
        /// <returns>A <see cref="ContentResult"/> containing the result of the insertion operation serialized as JSON.</returns>
        /// 
        [HttpPost("insertEmployee")]
        public async Task<ContentResult?> InsertEmployee([FromBody] InsertRequest objAPIRequest)
        {

            Response<object>? objAPIResponse = null;
            //var HeaderType = Request.ContentType;
            NexusBAL? objNexusBAL = null;
            ContentResult? objContentResult = null;
            object? objResult = null;
            int intStatusCode = 0;

            Response<object>? objResponse = new()
            {
                ReturnCode = Convert.ToInt32((int)General.CommonResponseErrorCodes.InvalidRequest),
                ReturnMessage = Enum.GetName(typeof(General.CommonResponseErrorCodes), General.CommonResponseErrorCodes.InvalidRequest)
            };

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

            objContentResult = new ContentResult() { Content = JsonConvert.SerializeObject(objResult), ContentType = "application/json", StatusCode = intStatusCode };
            return objContentResult;
        }

        /// <summary>
        /// Updates an employee record by ID.
        /// </summary>
        /// <param name="objAPIRequest">Updated employee data.</param>
        /// <returns>JSON result with update status.</returns>
        /// 
        // CHANGED: Changed from [HttpPost("updateEmployee")] to [HttpPut("updateEmployee")]
        [HttpPut("updateEmployee")]
        [Authorize(Policy = "Admin")]
        public async Task<ContentResult?> UpdateEmployeeById([FromBody] UpdateRequest objAPIRequest)
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

            objContentResult = new ContentResult() { Content = JsonConvert.SerializeObject(objResult), ContentType = "application/json", StatusCode = intStatusCode };
            return objContentResult;
        }

        /// <summary>
        /// Deletes an employee record based on the provided ID in the request payload.
        /// </summary>
        /// <param name="objAPIRequest">The request object containing the target employee ID and client metadata.</param>
        /// <returns>A <see cref="ContentResult"/> containing the outcome of the delete operation serialized as JSON.</returns>
        /// 
        // CHANGED: Changed from [HttpPost("deleteEmployee")] to [HttpDelete("deleteEmployee")]
        [HttpDelete("deleteEmployee")]
        [Authorize(Policy = "Admin")]
        public async Task<ContentResult?> DeleteEmployeeById([FromBody] DeleteRequest objAPIRequest)
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
