CREATE OR ALTER PROCEDURE dbo.usp_InsertEmployee
(
    @FirstName           NVARCHAR(64),
    @MiddleName          NVARCHAR(64) = NULL,
    @LastName            NVARCHAR(64) = NULL,
    @Email               NVARCHAR(128),
    @Cmail               NVARCHAR(128),
    @MobileNo            NVARCHAR(16),
    @Gender              NVARCHAR(8),
    @EmployeeType        NVARCHAR(32),
    @DateOfJoining       DATE = NULL,
    @DepartmentId        BIGINT,
    @DesignationId       BIGINT,
    @ReportingManagerId  BIGINT = NULL,
    @CurrentSalary       DECIMAL(18,2),
    @CompanyId           BIGINT,
    @SiteId              BIGINT,
    @Tag                 NVARCHAR(64) = NULL,
    @Comments            NVARCHAR(512) = NULL,
    @IPAddress           NVARCHAR(64),
    @CreatedBy           NVARCHAR(128),
    @DateCreated         DATETIME2 = NULL
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @ExistingGuId NVARCHAR(64);

    SELECT TOP (1) @ExistingGuId = Id
    FROM dbo.tblEmployees
    WHERE IsDeleted = 0
      AND
      (
          Email = @Email
          OR Cmail = @Cmail
          OR MobileNo = @MobileNo
      );

    IF @ExistingGuId IS NOT NULL
    BEGIN
        SELECT @ExistingGuId AS GuId, 33 AS ErrorCode,
               'Employee profile with this Personal Email, Company Email, or Mobile Number already exists.' AS ErrorMessage;
        RETURN;
    END;

    SET @DateOfJoining = ISNULL(@DateOfJoining, CAST(GETDATE() AS DATE));
    SET @DateCreated = ISNULL(@DateCreated, GETDATE());

    DECLARE @NextNum INT;
    DECLARE @EmployeeCode NVARCHAR(64);

    SELECT @NextNum =
        ISNULL(MAX(TRY_CAST(SUBSTRING(EmployeeCode, 4, 10) AS INT)), 0) + 1
    FROM dbo.tblEmployees
    WHERE EmployeeCode LIKE 'EMP[0-9]%';

    SET @EmployeeCode =
        'EMP' +
        CASE
            WHEN @NextNum < 1000
                THEN RIGHT('000' + CAST(@NextNum AS VARCHAR(10)), 3)
            ELSE CAST(@NextNum AS VARCHAR(10))
        END;

    INSERT INTO dbo.tblEmployees
    (
        EmployeeCode, FirstName, MiddleName, LastName,
        Email, Cmail, MobileNo, Gender, EmployeeType,
        DateOfJoining, DepartmentId, DesignationId,
        ReportingManagerId, CurrentSalary, CompanyId, SiteId,
        Tag, Comments, IPAddress, CreatedBy, DateCreated
    )
    OUTPUT inserted.Id AS GuId, 0 AS ErrorCode, 'Success' AS ErrorMessage
    VALUES
    (
        @EmployeeCode, @FirstName, @MiddleName, @LastName,
        @Email, @Cmail, @MobileNo, @Gender, @EmployeeType,
        @DateOfJoining, @DepartmentId, @DesignationId,
        @ReportingManagerId, @CurrentSalary, @CompanyId, @SiteId,
        @Tag, @Comments, @IPAddress, @CreatedBy, @DateCreated
    );
END
GO
