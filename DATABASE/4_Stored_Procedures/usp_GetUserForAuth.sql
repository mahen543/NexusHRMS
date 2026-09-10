CREATE OR ALTER PROCEDURE dbo.usp_GetUserForAuth
(
    @Username NVARCHAR(64)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        U.UserId,
        U.UserName AS Username,
        U.PasswordHash,
        E.Id AS EmployeeGuid,
        E.EmployeeId,
        E.EmployeeCode,
        E.DesignationId,
        D.DesignationName,
        D.HierarchyLevel,
        D.CanViewSalary,
        D.CanApproveHike,
        D.CanManageEmployees,
        E.ReportingManagerId
    FROM dbo.tblUsers U
    INNER JOIN dbo.tblEmployees E
        ON U.EmployeeId = E.EmployeeId
    INNER JOIN dbo.tblDesignations D
        ON E.DesignationId = D.DesignationId
    WHERE U.UserName = @Username
      AND U.IsActive = 1
      AND U.IsDeleted = 0
      AND E.IsDeleted = 0
      AND D.IsDeleted = 0;
END
GO
