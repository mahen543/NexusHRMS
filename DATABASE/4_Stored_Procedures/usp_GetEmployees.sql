CREATE OR ALTER PROCEDURE dbo.usp_GetEmployees
(
    @SearchWord NVARCHAR(100) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    SET @SearchWord = '%' + ISNULL(@SearchWord, '') + '%';

    SELECT
        E.EmployeeId,
        E.Id AS GuId,
        E.EmployeeCode,
        E.FirstName,
        E.MiddleName,
        E.LastName,
        CONCAT(E.FirstName, ' ', ISNULL(E.MiddleName + ' ', ''), E.LastName) AS FullName,
        E.Email,
        E.Cmail,
        E.MobileNo,
        E.Gender,
        E.DateOfJoining,
        E.EmployeeType,
        E.CurrentSalary,
        E.ExperienceYears,
        C.CompanyName,
        S.SiteName,
        D.DepartmentName,
        DG.DesignationName,
        DG.HierarchyLevel,
        DG.CanViewSalary,
        DG.CanApproveHike,
        DG.CanManageEmployees,
        RM.EmployeeCode AS ReportingManagerCode,
        CONCAT(RM.FirstName, ' ', RM.LastName) AS ReportingManagerName,
        E.IsPublished,
        E.DisplayOnWeb,
        E.IsDeleted,
        E.DateCreated
    FROM dbo.tblEmployees E
    INNER JOIN dbo.tblCompanies C ON E.CompanyId = C.CompanyId
    INNER JOIN dbo.tblSites S ON E.SiteId = S.SiteId
    INNER JOIN dbo.tblDepartments D ON E.DepartmentId = D.DepartmentId
    INNER JOIN dbo.tblDesignations DG ON E.DesignationId = DG.DesignationId
    LEFT JOIN dbo.tblEmployees RM ON E.ReportingManagerId = RM.EmployeeId
    WHERE
        E.IsDeleted = 0
        AND
        (
            E.EmployeeCode LIKE @SearchWord
            OR E.FirstName LIKE @SearchWord
            OR E.MiddleName LIKE @SearchWord
            OR E.LastName LIKE @SearchWord
            OR CONCAT(E.FirstName, ' ', ISNULL(E.MiddleName + ' ', ''), E.LastName) LIKE @SearchWord
            OR E.Email LIKE @SearchWord
            OR E.Cmail LIKE @SearchWord
            OR E.MobileNo LIKE @SearchWord
            OR D.DepartmentName LIKE @SearchWord
            OR DG.DesignationName LIKE @SearchWord
            OR C.CompanyName LIKE @SearchWord
            OR S.SiteName LIKE @SearchWord
        )
    ORDER BY E.EmployeeId DESC;
END
GO
