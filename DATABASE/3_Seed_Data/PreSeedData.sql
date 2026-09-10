/*
===============================================================================
NexusHRMS - Enterprise Seed Data
===============================================================================
Purpose:
    Development/demo data for NexusHRMS.

Important:
    - Identity primary keys are intentionally NOT supplied.
    - SQL Server generates EmployeeId, UserId, ProjectId, etc.
    - GUID-style Id columns are also intentionally NOT supplied; their
      DEFAULT(NEWID()) constraints generate them.
    - Foreign keys are resolved through stable business keys (codes/usernames)
      instead of hard-coded identity values.
    - The data demonstrates the intended authorization hierarchy and the
      performance -> salary hike -> manager approval workflow.

Seed order:
    01 Review Cycles
    02 Companies / Sites / Departments / Designations / Error Codes
    03 Employees (top-level first, then reporting hierarchy)
    04 Users
    05 Projects
    06 Employee Project Assignments
    07 Attendance
    08 HR Behavior Ratings
    09 Project Manager Feedback
    10 Salary Hike Requests
    11 Salary Hike Approvals
    12 Salary History
    13 Audit Logs
===============================================================================
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

/*=============================================================================
  01. REVIEW CYCLES
=============================================================================*/
INSERT INTO dbo.tblReviewCycles
(
    ReviewCycleName,
    CycleStartDate,
    CycleEndDate,
    FinancialEffectiveDate,
    IsActive,
    CreatedBy,
    IPAddress
)
VALUES
(
    'FY2026 Annual Review',
    '2026-01-01',
    '2026-12-31',
    '2027-01-01',
    1,
    'Admin',
    '83.110.106.181'
);

/*=============================================================================
  02. ORGANIZATION MASTER DATA
=============================================================================*/
INSERT INTO dbo.tblCompanies
(
    CompanyCode,
    CompanyName,
    WebsiteUrl,
    Comments
)
VALUES
('NEXUS', 'Nexus Technologies LLC', 'https://example.com',
 'Primary demonstration company for NexusHRMS');

/* Resolve the generated CompanyId. */
DECLARE @CompanyId BIGINT =
(
    SELECT CompanyId
    FROM dbo.tblCompanies
    WHERE CompanyCode = 'NEXUS'
);

/* Sites */
INSERT INTO dbo.tblSites
(
    CompanyId,
    SiteCode,
    SiteName,
    City,
    CountryName,
    ZipCode
)
VALUES
(@CompanyId, 'DXB-HQ', 'Dubai Headquarters', 'Dubai', 'UAE', '000001'),
(@CompanyId, 'AUH-DEV', 'Abu Dhabi Development Center', 'Abu Dhabi', 'UAE', '000002');

/* Departments */
DECLARE @DubaiSiteId BIGINT =
(
    SELECT SiteId
    FROM dbo.tblSites
    WHERE CompanyId = @CompanyId
      AND SiteCode = 'DXB-HQ'
);

DECLARE @AbuDhabiSiteId BIGINT =
(
    SELECT SiteId
    FROM dbo.tblSites
    WHERE CompanyId = @CompanyId
      AND SiteCode = 'AUH-DEV'
);

INSERT INTO dbo.tblDepartments
(
    SiteId,
    DepartmentCode,
    DepartmentName
)
VALUES
(@DubaiSiteId, 'HR',  'Human Resources'),
(@DubaiSiteId, 'ENG', 'Engineering'),
(@DubaiSiteId, 'FIN', 'Finance'),
(@AbuDhabiSiteId, 'QA', 'Quality Assurance');

/* Designations / policy capabilities
   Manager is the compensation decision maker.
   Project Manager supplies project/performance feedback but does NOT approve
   salary hikes and does NOT receive salary visibility from this designation.
*/
INSERT INTO dbo.tblDesignations
(
    DesignationCode,
    DesignationName,
    HierarchyLevel,
    CanViewSalary,
    CanApproveHike,
    CanManageEmployees
)
VALUES
('ADMIN', 'System Administrator', 1, 1, 0, 1),
('HRM',   'HR Manager',           2, 1, 0, 1),
('MGR',   'Manager',              3, 1, 1, 0),
('PM',    'Project Manager',      3, 0, 0, 0),
('TL',    'Team Lead',            4, 0, 0, 1),
('SE',    'Software Engineer',    5, 0, 0, 0),
('QA',    'QA Engineer',          5, 0, 0, 0),
('ACC',   'Accountant',           4, 1, 0, 0);

/* Error codes required by the employee stored procedures.
   ErrorCode is the natural primary key and is NOT an IDENTITY column. */
INSERT INTO dbo.tblErrorCode
(
    ErrorCode,
    ErrorName,
    ErrorMessage,
    CompanyId,
    SiteId
)
VALUES
(0,   'SUCCESS',          'Operation completed successfully.', @CompanyId, @DubaiSiteId),
(33,  'DUPLICATE_RECORD', 'Personal Email, Company Email, or Mobile Number is already in use.', @CompanyId, @DubaiSiteId),
(404, 'NOT_FOUND',        'Requested record was not found or is inactive/deleted.', @CompanyId, @DubaiSiteId);

/*=============================================================================
  03. EMPLOYEES
  Insert reporting roots first because ReportingManagerId is self-referencing.
=============================================================================*/

DECLARE
    @HRDesignationId BIGINT = (SELECT DesignationId FROM dbo.tblDesignations WHERE DesignationCode = 'HRM'),
    @ManagerDesignationId BIGINT = (SELECT DesignationId FROM dbo.tblDesignations WHERE DesignationCode = 'MGR'),
    @PMDesignationId BIGINT = (SELECT DesignationId FROM dbo.tblDesignations WHERE DesignationCode = 'PM'),
    @TLDesignationId BIGINT = (SELECT DesignationId FROM dbo.tblDesignations WHERE DesignationCode = 'TL'),
    @SEDesignationId BIGINT = (SELECT DesignationId FROM dbo.tblDesignations WHERE DesignationCode = 'SE'),
    @QADesignationId BIGINT = (SELECT DesignationId FROM dbo.tblDesignations WHERE DesignationCode = 'QA'),
    @ACCDesignationId BIGINT = (SELECT DesignationId FROM dbo.tblDesignations WHERE DesignationCode = 'ACC'),
    @AdminDesignationId BIGINT = (SELECT DesignationId FROM dbo.tblDesignations WHERE DesignationCode = 'ADMIN');

DECLARE
    @HRDepartmentId BIGINT = (SELECT DepartmentId FROM dbo.tblDepartments WHERE DepartmentCode = 'HR'),
    @EngineeringDepartmentId BIGINT = (SELECT DepartmentId FROM dbo.tblDepartments WHERE DepartmentCode = 'ENG'),
    @FinanceDepartmentId BIGINT = (SELECT DepartmentId FROM dbo.tblDepartments WHERE DepartmentCode = 'FIN'),
    @QADepartmentId BIGINT = (SELECT DepartmentId FROM dbo.tblDepartments WHERE DepartmentCode = 'QA');

DECLARE @FY2026ReviewCycleId BIGINT =
(
    SELECT ReviewCycleId
    FROM dbo.tblReviewCycles
    WHERE ReviewCycleName = 'FY2026 Annual Review'
);

/* Top-level administration */
INSERT INTO dbo.tblEmployees
(
    EmployeeCode, FirstName, LastName, Email, Cmail, MobileNo, Gender,
    EmployeeType, DateOfJoining, DepartmentId, DesignationId,
    ReportingManagerId, CurrentSalary, ExperienceYears,
    CompanyId, SiteId, IPAddress, CreatedBy
)
VALUES
(
    'EMP-ADMIN', 'Aarav', 'Sharma',
    'aarav.sharma@nexus.local', 'aarav.sharma@nexus.local', '971500000001',
    'Male', 'Full-Time', '2020-01-10', @HRDepartmentId, @AdminDesignationId,
    NULL, 30000.00, 10.00, @CompanyId, @DubaiSiteId,
    '83.110.106.181', 'Admin'
);

/* HR manager reports to administrator */
INSERT INTO dbo.tblEmployees
(
    EmployeeCode, FirstName, LastName, Email, Cmail, MobileNo, Gender,
    EmployeeType, DateOfJoining, DepartmentId, DesignationId,
    ReportingManagerId, CurrentSalary, ExperienceYears,
    CompanyId, SiteId, IPAddress, CreatedBy
)
SELECT
    'EMP-HR-001', 'Fatima', 'Khan',
    'fatima.khan@nexus.local', 'fatima.khan@nexus.local', '971500000002',
    'Female', 'Full-Time', '2021-03-15', @HRDepartmentId, @HRDesignationId,
    E.EmployeeId, 20000.00, 7.00,
    @CompanyId, @DubaiSiteId, '83.110.106.181', 'Admin'
FROM dbo.tblEmployees E
WHERE E.EmployeeCode = 'EMP-ADMIN';

/* Manager reports to administrator */
INSERT INTO dbo.tblEmployees
(
    EmployeeCode, FirstName, LastName, Email, Cmail, MobileNo, Gender,
    EmployeeType, DateOfJoining, DepartmentId, DesignationId,
    ReportingManagerId, CurrentSalary, ExperienceYears,
    CompanyId, SiteId, IPAddress, CreatedBy
)
SELECT
    'EMP-MGR-001', 'Omar', 'Hassan',
    'omar.hassan@nexus.local', 'omar.hassan@nexus.local', '971500000003',
    'Male', 'Full-Time', '2021-06-01', @EngineeringDepartmentId, @ManagerDesignationId,
    E.EmployeeId, 22000.00, 8.00,
    @CompanyId, @DubaiSiteId, '83.110.106.181', 'Admin'
FROM dbo.tblEmployees E
WHERE E.EmployeeCode = 'EMP-ADMIN';

/* Project Manager reports to manager */
INSERT INTO dbo.tblEmployees
(
    EmployeeCode, FirstName, LastName, Email, Cmail, MobileNo, Gender,
    EmployeeType, DateOfJoining, DepartmentId, DesignationId,
    ReportingManagerId, CurrentSalary, ExperienceYears,
    CompanyId, SiteId, IPAddress, CreatedBy
)
SELECT
    'EMP-PM-001', 'Rahul', 'Verma',
    'rahul.verma@nexus.local', 'rahul.verma@nexus.local', '971500000004',
    'Male', 'Full-Time', '2021-06-01', @EngineeringDepartmentId, @PMDesignationId,
    E.EmployeeId, 19000.00, 8.00,
    @CompanyId, @DubaiSiteId, '83.110.106.181', 'Admin'
FROM dbo.tblEmployees E
WHERE E.EmployeeCode = 'EMP-MGR-001';

/* Team Lead reports to manager */
INSERT INTO dbo.tblEmployees
(
    EmployeeCode, FirstName, LastName, Email, Cmail, MobileNo, Gender,
    EmployeeType, DateOfJoining, DepartmentId, DesignationId,
    ReportingManagerId, CurrentSalary, ExperienceYears,
    CompanyId, SiteId, IPAddress, CreatedBy
)
SELECT
    'EMP-TL-001', 'Sneha', 'Iyer',
    'sneha.iyer@nexus.local', 'sneha.iyer@nexus.local', '971500000005',
    'Female', 'Full-Time', '2022-01-12', @EngineeringDepartmentId, @TLDesignationId,
    E.EmployeeId, 15500.00, 5.00,
    @CompanyId, @DubaiSiteId, '83.110.106.181', 'Admin'
FROM dbo.tblEmployees E
WHERE E.EmployeeCode = 'EMP-MGR-001';

/* Engineering employee reports to Team Lead */
INSERT INTO dbo.tblEmployees
(
    EmployeeCode, FirstName, LastName, Email, Cmail, MobileNo, Gender,
    EmployeeType, DateOfJoining, DepartmentId, DesignationId,
    ReportingManagerId, CurrentSalary, ExperienceYears,
    CompanyId, SiteId, IPAddress, CreatedBy
)
SELECT
    'EMP-SE-001', 'Mohammed', 'Ali',
    'mohammed.ali@nexus.local', 'mohammed.ali@nexus.local', '971500000006',
    'Male', 'Full-Time', '2022-05-18', @EngineeringDepartmentId, @SEDesignationId,
    E.EmployeeId, 9000.00, 3.00,
    @CompanyId, @DubaiSiteId, '83.110.106.181', 'Admin'
FROM dbo.tblEmployees E
WHERE E.EmployeeCode = 'EMP-TL-001';

/* QA employee reports to Team Lead */
INSERT INTO dbo.tblEmployees
(
    EmployeeCode, FirstName, LastName, Email, Cmail, MobileNo, Gender,
    EmployeeType, DateOfJoining, DepartmentId, DesignationId,
    ReportingManagerId, CurrentSalary, ExperienceYears,
    CompanyId, SiteId, IPAddress, CreatedBy
)
SELECT
    'EMP-QA-001', 'Priya', 'Reddy',
    'priya.reddy@nexus.local', 'priya.reddy@nexus.local', '971500000007',
    'Female', 'Full-Time', '2023-02-20', @QADepartmentId, @QADesignationId,
    E.EmployeeId, 8500.00, 2.00,
    @CompanyId, @AbuDhabiSiteId, '83.110.106.181', 'Admin'
FROM dbo.tblEmployees E
WHERE E.EmployeeCode = 'EMP-TL-001';

/* Finance employee reports to administrator */
INSERT INTO dbo.tblEmployees
(
    EmployeeCode, FirstName, LastName, Email, Cmail, MobileNo, Gender,
    EmployeeType, DateOfJoining, DepartmentId, DesignationId,
    ReportingManagerId, CurrentSalary, ExperienceYears,
    CompanyId, SiteId, IPAddress, CreatedBy
)
SELECT
    'EMP-ACC-001', 'Daniel', 'George',
    'daniel.george@nexus.local', 'daniel.george@nexus.local', '971500000008',
    'Male', 'Full-Time', '2021-11-10', @FinanceDepartmentId, @ACCDesignationId,
    E.EmployeeId, 12500.00, 6.00,
    @CompanyId, @DubaiSiteId, '83.110.106.181', 'Admin'
FROM dbo.tblEmployees E
WHERE E.EmployeeCode = 'EMP-ADMIN';

/*=============================================================================
  04. USERS
  NOTE:
    PasswordHash values below are intentionally placeholders. Replace them with
    hashes generated by the same password-hashing mechanism used by the API.
    No UserId/EmployeeId is supplied; both are resolved/generated by SQL Server.
=============================================================================*/

INSERT INTO dbo.tblUsers
(
    EmployeeId,
    UserName,
    PasswordHash,
    IsActive,
    IPAddress
)
SELECT E.EmployeeId, 'admin', 'REPLACE_WITH_REAL_PASSWORD_HASH', 1, '83.110.106.181'
FROM dbo.tblEmployees E
WHERE E.EmployeeCode = 'EMP-ADMIN';

INSERT INTO dbo.tblUsers
(
    EmployeeId,
    UserName,
    PasswordHash,
    IsActive,
    IPAddress
)
SELECT E.EmployeeId, 'hr.manager', 'REPLACE_WITH_REAL_PASSWORD_HASH', 1, '83.110.106.181'
FROM dbo.tblEmployees E
WHERE E.EmployeeCode = 'EMP-HR-001';

INSERT INTO dbo.tblUsers
(
    EmployeeId,
    UserName,
    PasswordHash,
    IsActive,
    IPAddress
)
SELECT E.EmployeeId, 'manager', 'REPLACE_WITH_REAL_PASSWORD_HASH', 1, '83.110.106.181'
FROM dbo.tblEmployees E
WHERE E.EmployeeCode = 'EMP-MGR-001';

INSERT INTO dbo.tblUsers
(
    EmployeeId,
    UserName,
    PasswordHash,
    IsActive,
    IPAddress
)
SELECT E.EmployeeId, 'project.manager', 'REPLACE_WITH_REAL_PASSWORD_HASH', 1, '83.110.106.181'
FROM dbo.tblEmployees E
WHERE E.EmployeeCode = 'EMP-PM-001';

INSERT INTO dbo.tblUsers
(
    EmployeeId,
    UserName,
    PasswordHash,
    IsActive,
    IPAddress
)
SELECT E.EmployeeId, 'team.lead', 'REPLACE_WITH_REAL_PASSWORD_HASH', 1, '83.110.106.181'
FROM dbo.tblEmployees E
WHERE E.EmployeeCode = 'EMP-TL-001';

INSERT INTO dbo.tblUsers
(
    EmployeeId,
    UserName,
    PasswordHash,
    IsActive,
    IPAddress
)
SELECT E.EmployeeId, 'software.engineer', 'REPLACE_WITH_REAL_PASSWORD_HASH', 1, '83.110.106.181'
FROM dbo.tblEmployees E
WHERE E.EmployeeCode = 'EMP-SE-001';

INSERT INTO dbo.tblUsers
(
    EmployeeId,
    UserName,
    PasswordHash,
    IsActive,
    IPAddress
)
SELECT E.EmployeeId, 'qa.engineer', 'REPLACE_WITH_REAL_PASSWORD_HASH', 1, '83.110.106.181'
FROM dbo.tblEmployees E
WHERE E.EmployeeCode = 'EMP-QA-001';

INSERT INTO dbo.tblUsers
(
    EmployeeId,
    UserName,
    PasswordHash,
    IsActive,
    IPAddress
)
SELECT E.EmployeeId, 'accountant', 'REPLACE_WITH_REAL_PASSWORD_HASH', 1, '83.110.106.181'
FROM dbo.tblEmployees E
WHERE E.EmployeeCode = 'EMP-ACC-001';

/*=============================================================================
  05. PROJECTS
=============================================================================*/
INSERT INTO dbo.tblProjects
(
    ProjectCode,
    ProjectName,
    ClientName,
    DepartmentId,
    ProjectManagerId,
    StartDate,
    EndDate,
    CompanyId,
    IsActive,
    CreatedBy,
    IPAddress
)
SELECT
    'BANK-CRM',
    'Banking CRM Platform',
    'Emirates Bank',
    @EngineeringDepartmentId,
    E.EmployeeId,
    '2026-01-01',
    NULL,
    @CompanyId,
    1,
    'Admin',
    '83.110.106.181'
FROM dbo.tblEmployees E
WHERE E.EmployeeCode = 'EMP-PM-001';

INSERT INTO dbo.tblProjects
(
    ProjectCode,
    ProjectName,
    ClientName,
    DepartmentId,
    ProjectManagerId,
    StartDate,
    EndDate,
    CompanyId,
    IsActive,
    CreatedBy,
    IPAddress
)
SELECT
    'ERP-CORE',
    'Enterprise ERP System',
    'Global Holdings',
    @EngineeringDepartmentId,
    E.EmployeeId,
    '2026-02-01',
    NULL,
    @CompanyId,
    1,
    'Admin',
    '83.110.106.181'
FROM dbo.tblEmployees E
WHERE E.EmployeeCode = 'EMP-PM-001';

/*=============================================================================
  06. PROJECT ASSIGNMENTS
=============================================================================*/
DECLARE @BankCrmProjectId BIGINT =
(
    SELECT ProjectId FROM dbo.tblProjects
    WHERE ProjectCode = 'BANK-CRM' AND CompanyId = @CompanyId
);

INSERT INTO dbo.tblEmployeeProjects
(
    EmployeeId,
    ProjectId,
    ReportingProjectManagerId,
    RoleInProject,
    StartDate,
    EndDate,
    IsActive,
    CreatedBy
)
SELECT
    E.EmployeeId,
    @BankCrmProjectId,
    PM.EmployeeId,
    'Backend Developer',
    '2026-01-01',
    NULL,
    1,
    'Admin'
FROM dbo.tblEmployees E
CROSS JOIN dbo.tblEmployees PM
WHERE E.EmployeeCode = 'EMP-SE-001'
  AND PM.EmployeeCode = 'EMP-PM-001';

INSERT INTO dbo.tblEmployeeProjects
(
    EmployeeId,
    ProjectId,
    ReportingProjectManagerId,
    RoleInProject,
    StartDate,
    EndDate,
    IsActive,
    CreatedBy
)
SELECT
    E.EmployeeId,
    @BankCrmProjectId,
    PM.EmployeeId,
    'Technical Lead',
    '2026-01-01',
    NULL,
    1,
    'Admin'
FROM dbo.tblEmployees E
CROSS JOIN dbo.tblEmployees PM
WHERE E.EmployeeCode = 'EMP-TL-001'
  AND PM.EmployeeCode = 'EMP-PM-001';

/*=============================================================================
  07. ATTENDANCE
=============================================================================*/
DECLARE @HRId BIGINT =
(
    SELECT EmployeeId FROM dbo.tblEmployees WHERE EmployeeCode = 'EMP-HR-001'
);

INSERT INTO dbo.tblAttendance
(
    EmployeeId,
    AttendanceDate,
    AttendanceStatus,
    Remarks,
    MarkedByEmployeeId,
    IsActive,
    CreatedBy
)
SELECT E.EmployeeId, '2026-05-01', 'Present', 'On time', @HRId, 1, 'Admin'
FROM dbo.tblEmployees E
WHERE E.EmployeeCode = 'EMP-SE-001';

INSERT INTO dbo.tblAttendance
(
    EmployeeId,
    AttendanceDate,
    AttendanceStatus,
    Remarks,
    MarkedByEmployeeId,
    IsActive,
    CreatedBy
)
SELECT E.EmployeeId, '2026-05-02', 'WFH', 'Remote work', @HRId, 1, 'Admin'
FROM dbo.tblEmployees E
WHERE E.EmployeeCode = 'EMP-SE-001';

INSERT INTO dbo.tblAttendance
(
    EmployeeId,
    AttendanceDate,
    AttendanceStatus,
    Remarks,
    MarkedByEmployeeId,
    IsActive,
    CreatedBy
)
SELECT E.EmployeeId, '2026-05-01', 'Present', 'Regular shift', @HRId, 1, 'Admin'
FROM dbo.tblEmployees E
WHERE E.EmployeeCode = 'EMP-QA-001';

INSERT INTO dbo.tblAttendance
(
    EmployeeId,
    AttendanceDate,
    AttendanceStatus,
    Remarks,
    MarkedByEmployeeId,
    IsActive,
    CreatedBy
)
SELECT E.EmployeeId, '2026-05-02', 'Absent', 'Unplanned absence', @HRId, 1, 'Admin'
FROM dbo.tblEmployees E
WHERE E.EmployeeCode = 'EMP-QA-001';

/*=============================================================================
  08. HR BEHAVIOR RATINGS
=============================================================================*/
INSERT INTO dbo.tblBehaviorRatings
(
    EmployeeId,
    ReviewCycleId,
    CommunicationScore,
    TeamworkScore,
    DisciplineScore,
    OverallScore,
    HRRemarks,
    RatedByEmployeeId
)
SELECT
    E.EmployeeId,
    @FY2026ReviewCycleId,
    8.50, 9.00, 8.00, 8.50,
    'Strong collaboration and consistent workplace discipline.',
    @HRId
FROM dbo.tblEmployees E
WHERE E.EmployeeCode = 'EMP-SE-001';

INSERT INTO dbo.tblBehaviorRatings
(
    EmployeeId,
    ReviewCycleId,
    CommunicationScore,
    TeamworkScore,
    DisciplineScore,
    OverallScore,
    HRRemarks,
    RatedByEmployeeId
)
SELECT
    E.EmployeeId,
    @FY2026ReviewCycleId,
    8.00, 8.50, 9.00, 8.50,
    'Reliable and disciplined with strong team contribution.',
    @HRId
FROM dbo.tblEmployees E
WHERE E.EmployeeCode = 'EMP-QA-001';

/*=============================================================================
  09. PROJECT MANAGER FEEDBACK
=============================================================================*/
DECLARE @PMId BIGINT =
(
    SELECT EmployeeId FROM dbo.tblEmployees WHERE EmployeeCode = 'EMP-PM-001'
);

INSERT INTO dbo.tblProjectManagerFeedback
(
    EmployeeId,
    ProjectId,
    ReviewCycleId,
    TechnicalScore,
    DeliveryScore,
    ProblemSolvingScore,
    OverallScore,
    Feedback,
    ReviewedByProjectManagerEmployeeId
)
SELECT
    E.EmployeeId,
    @BankCrmProjectId,
    @FY2026ReviewCycleId,
    8.50, 8.00, 9.00, 8.50,
    'Strong backend engineering skills and effective problem solving.',
    @PMId
FROM dbo.tblEmployees E
WHERE E.EmployeeCode = 'EMP-SE-001';

/*=============================================================================
  10. SALARY HIKE REQUESTS
  Two scenarios demonstrate the workflow:
      A. Pending request awaiting Manager decision.
      B. Approved request with Manager decision.
=============================================================================*/
DECLARE @SEId BIGINT =
(
    SELECT EmployeeId FROM dbo.tblEmployees WHERE EmployeeCode = 'EMP-SE-001'
);

DECLARE @QAId BIGINT =
(
    SELECT EmployeeId FROM dbo.tblEmployees WHERE EmployeeCode = 'EMP-QA-001'
);

DECLARE @ManagerId BIGINT =
(
    SELECT EmployeeId FROM dbo.tblEmployees WHERE EmployeeCode = 'EMP-MGR-001'
);

DECLARE @SECurrentSalary DECIMAL(18,2) =
(
    SELECT CurrentSalary FROM dbo.tblEmployees WHERE EmployeeId = @SEId
);

DECLARE @QACurrentSalary DECIMAL(18,2) =
(
    SELECT CurrentSalary FROM dbo.tblEmployees WHERE EmployeeId = @QAId
);

/* Pending request */
INSERT INTO dbo.tblSalaryHikeRequests
(
    EmployeeId,
    ReviewCycleId,
    CurrentSalary,
    SuggestedHikePercent,
    SuggestedSalary,
    AverageHRScore,
    AverageProjectManagerScore,
    Status
)
VALUES
(
    @SEId,
    @FY2026ReviewCycleId,
    @SECurrentSalary,
    12.00,
    @SECurrentSalary * 1.12,
    8.50,
    8.50,
    'Pending'
);

/* Approved request */
INSERT INTO dbo.tblSalaryHikeRequests
(
    EmployeeId,
    ReviewCycleId,
    CurrentSalary,
    SuggestedHikePercent,
    SuggestedSalary,
    AverageHRScore,
    AverageProjectManagerScore,
    Status
)
VALUES
(
    @QAId,
    @FY2026ReviewCycleId,
    @QACurrentSalary,
    10.00,
    @QACurrentSalary * 1.10,
    8.50,
    8.50,
    'Approved'
);

/*=============================================================================
  11. MANAGER SALARY HIKE APPROVAL
  Manager is intentionally the approver; Admin/Project Manager are not used
  for the compensation decision in this demonstration data.
=============================================================================*/
DECLARE @ApprovedRequestId BIGINT =
(
    SELECT TOP (1) SalaryHikeRequestId
    FROM dbo.tblSalaryHikeRequests
    WHERE EmployeeId = @QAId
      AND ReviewCycleId = @FY2026ReviewCycleId
      AND Status = 'Approved'
    ORDER BY SalaryHikeRequestId DESC
);

INSERT INTO dbo.tblSalaryHikeApprovals
(
    SalaryHikeRequestId,
    ApproverEmployeeId,
    ApprovedHikePercent,
    FinalSalary,
    Decision,
    DecisionRemarks
)
SELECT
    @ApprovedRequestId,
    @ManagerId,
    10.00,
    SR.CurrentSalary * 1.10,
    'Approved',
    'Approved by Manager based on combined HR and Project Manager performance scores.'
FROM dbo.tblSalaryHikeRequests SR
WHERE SR.SalaryHikeRequestId = @ApprovedRequestId;

/*=============================================================================
  12. SALARY HISTORY
=============================================================================*/
INSERT INTO dbo.tblSalaryHistory
(
    EmployeeId,
    SalaryHikeRequestId,
    EffectiveFrom,
    OldSalary,
    NewSalary,
    HikePercent
)
SELECT
    SR.EmployeeId,
    SR.SalaryHikeRequestId,
    RC.FinancialEffectiveDate,
    SR.CurrentSalary,
    SHA.FinalSalary,
    SHA.ApprovedHikePercent
FROM dbo.tblSalaryHikeRequests SR
INNER JOIN dbo.tblSalaryHikeApprovals SHA
    ON SHA.SalaryHikeRequestId = SR.SalaryHikeRequestId
INNER JOIN dbo.tblReviewCycles RC
    ON RC.ReviewCycleId = SR.ReviewCycleId
WHERE SR.SalaryHikeRequestId = @ApprovedRequestId
  AND SHA.Decision = 'Approved';

/*=============================================================================
  13. AUDIT LOGS
=============================================================================*/
DECLARE @ManagerUserId BIGINT =
(
    SELECT UserId FROM dbo.tblUsers WHERE UserName = 'manager'
);

INSERT INTO dbo.tblAuditLogs
(
    UserId,
    ActionType,
    TableName,
    RecordId,
    OldValues,
    NewValues
)
VALUES
(
    @ManagerUserId,
    'LOGIN',
    'tblUsers',
    @ManagerUserId,
    NULL,
    'Manager authenticated successfully'
),
(
    @ManagerUserId,
    'UPDATE',
    'tblSalaryHikeRequests',
    @ApprovedRequestId,
    'Status = Pending',
    'Status = Approved; Approved by Manager'
);

COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    THROW;
END CATCH;

SET NOCOUNT OFF;
