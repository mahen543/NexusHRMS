using System;
using System.Collections.Generic;
using System.Text;

namespace HRMS_Common.CommonProperties
{
    public class RegularExpressions
    {
        #region Properties
        public string? RegExSearchWord { get; set; } = string.Empty;
        public string? RegExForId { set; get; } = string.Empty;
        public string? RegExForReleaseYear { set; get; } = string.Empty;
        public string? RegExForReportName { set; get; } = string.Empty;
        public string? RegExForPageNumber { set; get; } = string.Empty;
        public string? RegExForPageSize { set; get; } = string.Empty;
        public string? RegExForName { set; get; } = string.Empty;
        public string? RegExTrueOrFalse { set; get; } = string.Empty;
        public string? RegExUserName { set; get; } = string.Empty;
        public string? SecretKey { get; set; } = string.Empty;
        public string? RegExForMobileNumber { get; set; } = string.Empty;
        public string? RegExForEmail { get; set; } = string.Empty;

        #endregion
    }
}
