using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace HRMS_Common
{
    public class General
    {

        public enum ErrorCode
        {
            // Common
            Technical_Error = 1,
            No_Records_Found = 2,

            // Employee
            Id_Required = 3,
            Invalid_Id = 4,

            FirstName_Required = 5,
            Invalid_FirstName = 6,

            LastName_Required = 7,
            Invalid_LastName = 8,

            Email_Required = 9,
            Invalid_Email = 32,

            Department_Required = 11,

            Designation_Required = 13,

            Company_Required = 15,
            Invalid_Company = 16,

            Site_Required = 17,
            Invalid_Site = 18,

            DateOfJoining_Required = 19,

            Invalid_SearchWord = 21,

            DeletedBy_Required = 22,
            CreatedBy_Required = 23,
            UpdatedBy_Required = 12,
            Invalid_DeletedBy = 27,
            Invalid_CreatedBy = 28,
            Invalid_UpdatedBy = 29,
            Invalid_IsPublished = 14,

            Record_Already_Deleted = 31,
            Record_Not_Found = 24,
            Mobile_Number_Required = 25,
            Invalid_Mobile_Number = 26,
        }
        #region CommonResponseErrorCodes
        public enum CommonResponseErrorCodes
        {
            //http status codes
            Success = 200,
            RequestTimeOut = 408,
            BadRequest = 400,
            UnAuthorized = 401,
            Forbidden = 403,
            NotAcceptable = 406,
            InvalidRequest = -101,
            No_Records_Found = 22
        }

        public static Dictionary<string, string> dictCommonResponse
        {
            get
            {
                Dictionary<string, string> dictCommonResponse = new Dictionary<string, string>
                {
                    { CommonResponseErrorCodes.Success.ToString(), "Success" },
                    { CommonResponseErrorCodes.RequestTimeOut.ToString(), "Request Time Out" },
                    { CommonResponseErrorCodes.BadRequest.ToString(), "Bad Request" },
                    { CommonResponseErrorCodes.UnAuthorized.ToString(), "UnAuthorized" },
                    { CommonResponseErrorCodes.Forbidden.ToString(), "Forbidden" },
                    { CommonResponseErrorCodes.NotAcceptable.ToString(), "Not Acceptable" },
                    { CommonResponseErrorCodes.InvalidRequest.ToString(), "Invalid Request" },
                    { CommonResponseErrorCodes.No_Records_Found.ToString(), "No Records Found" }
                };
                return dictCommonResponse;
            }
        }
        #endregion

        #region IpAddress
        public string GetIP4Address()
        {
            string IP4Address = String.Empty;

            foreach (IPAddress IPA in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (IPA.AddressFamily.ToString() == "InterNetwork")
                {
                    IP4Address = IPA.ToString();
                    break;
                }
            }
            return IP4Address;
        }
        #endregion
    }
}
