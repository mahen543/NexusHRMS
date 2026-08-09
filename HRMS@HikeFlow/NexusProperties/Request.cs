using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HRMS_HikeFlow.NexusProperties
{

    public class ChangePasswordRequestDto
    {
        public long? UserId { get; set; }
        public string? NewPassword { get; set; }
        public string? IPAddress { get; set; }
    }

    public class LoginRequestDto
    {
        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    // Database model mapping for user authentication details
    public class UserAuthModel
    {
        public long UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string EmployeeGuid { get; set; } = string.Empty;
        public long DesignationId { get; set; }
        public byte HierarchyLevel { get; set; }
    }

    // Immutable record model representing user hierarchy context for in-memory RBAC evaluation
    public record UserHierarchyModel(
        long UserId,
        string Username,
        byte HierarchyLevel,
        long DesignationId
    );


    public record GetRequest(
        
        string? SearchWord = null,
        int PageNumber = 1,
        int PageSize = 10
    );

    // 2. INSERT Request (Requires all data except IDs and dates handled by DB)
    public record InsertRequest(

        // Required Fields
        string FirstName,
        string Email,
        string MobileNo,
        string Gender,
        string EmployeeType,
        long DepartmentId,
        long DesignationId,
        decimal CurrentSalary,
        long CompanyId,
        long SiteId,
        string CreatedBy,

        // Optional Fields
        string? IPAddress = null,
        string? MiddleName = null,
        string? LastName = null,
        long? ReportingManagerId = null,
        string? Tag = null,
        string? Comments = null,
        DateTime? DateOfJoining = null
    );

    // 3. UPDATE Request (Requires an ID to find the record, plus updated details)
    public record UpdateRequest(
        
        string Id,
        string UpdatedBy,
        string? FirstName = null,
        string? LastName = null,
        string? MiddleName = null,
        string? Email = null,
        string? Cmail = null,
        string? MobileNo = null,
        string? Gender = null,
        long? DepartmentId = null,
        long? DesignationId = null,
        decimal? CurrentSalary = null,
        long? CompanyId = null,
        long? SiteId = null,
        string? EmployeeCode = null,
        string? EmployeeType = null,
        long? ReportingManagerId = null,
        decimal? ExperienceYears = null,
        bool? IsPublished = null,
        string? PublishedBy = null,
        bool? DisplayOnWeb = null,
        int? SortOrder = null,
        string? Tag = null,
        string? Comments = null,
        string? IPAddress = null,
        DateTime? DateOfJoining = null
    );

    // 4. DELETE Request (Only requires identification and audit trail)
    public record DeleteRequest(
        
        string Id,
        string DeletedBy,
        string? IPAddress = null
    );
}
