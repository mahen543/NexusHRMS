CREATE OR ALTER PROCEDURE dbo.usp_UpdateEmployee
(
    @Id                 NVARCHAR(64),
    @FirstName          NVARCHAR(64) = NULL,
    @MiddleName         NVARCHAR(64) = NULL,
    @LastName           NVARCHAR(64) = NULL,
    @Email              NVARCHAR(128) = NULL,
    @Cmail              NVARCHAR(128) = NULL,
    @MobileNo           NVARCHAR(16) = NULL,
    @Gender             NVARCHAR(8) = NULL,
    @DateOfJoining      DATE = NULL,
    @DepartmentId       BIGINT = NULL,
    @DesignationId      BIGINT = NULL,
    @EmployeeType       NVARCHAR(32) = NULL,
    @ReportingManagerId BIGINT = NULL,
    @CurrentSalary      DECIMAL(18,2) = NULL,
    @ExperienceYears    DECIMAL(5,2) = NULL,
    @CompanyId          BIGINT = NULL,
    @SiteId             BIGINT = NULL,
    @IsPublished        BIT = NULL,
    @PublishedBy        NVARCHAR(128) = NULL,
    @DatePublished      DATETIME2 = NULL,
    @DisplayOnWeb       BIT = NULL,
    @SortOrder          INT = NULL,
    @Tag                NVARCHAR(64) = NULL,
    @Comments           NVARCHAR(512) = NULL,
    @IPAddress          NVARCHAR(64) = NULL,
    @UpdatedBy          NVARCHAR(128)
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.tblEmployees
        WHERE Id = @Id
          AND IsDeleted = 0
    )
    BEGIN
        SELECT @Id AS GuId, 404 AS ErrorCode,
               'Employee record not found or has been deleted.' AS ErrorMessage;
        RETURN;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM dbo.tblEmployees
        WHERE Id <> @Id
          AND IsDeleted = 0
          AND
          (
              (@Email IS NOT NULL AND Email = @Email)
              OR (@Cmail IS NOT NULL AND Cmail = @Cmail)
              OR (@MobileNo IS NOT NULL AND MobileNo = @MobileNo)
          )
    )
    BEGIN
        SELECT @Id AS GuId, 33 AS ErrorCode,
               'Cannot update record. This Personal Email, Company Email, or Mobile Number is already in use by another employee.' AS ErrorMessage;
        RETURN;
    END;

    UPDATE dbo.tblEmployees
    SET
        FirstName = ISNULL(@FirstName, FirstName),
        MiddleName = ISNULL(@MiddleName, MiddleName),
        LastName = ISNULL(@LastName, LastName),
        Email = ISNULL(@Email, Email),
        Cmail = ISNULL(@Cmail, Cmail),
        MobileNo = ISNULL(@MobileNo, MobileNo),
        Gender = ISNULL(@Gender, Gender),
        DateOfJoining = ISNULL(@DateOfJoining, DateOfJoining),
        DepartmentId = ISNULL(@DepartmentId, DepartmentId),
        DesignationId = ISNULL(@DesignationId, DesignationId),
        EmployeeType = ISNULL(@EmployeeType, EmployeeType),
        ReportingManagerId = ISNULL(@ReportingManagerId, ReportingManagerId),
        CurrentSalary = ISNULL(@CurrentSalary, CurrentSalary),
        ExperienceYears = ISNULL(@ExperienceYears, ExperienceYears),
        CompanyId = ISNULL(@CompanyId, CompanyId),
        SiteId = ISNULL(@SiteId, SiteId),
        IsPublished = ISNULL(@IsPublished, IsPublished),
        PublishedBy = ISNULL(@PublishedBy, PublishedBy),
        DatePublished = ISNULL(@DatePublished, DatePublished),
        DisplayOnWeb = ISNULL(@DisplayOnWeb, DisplayOnWeb),
        SortOrder = ISNULL(@SortOrder, SortOrder),
        Tag = ISNULL(@Tag, Tag),
        Comments = ISNULL(@Comments, Comments),
        IPAddress = ISNULL(@IPAddress, IPAddress),
        UpdatedBy = @UpdatedBy,
        LastUpdated = GETDATE()
    WHERE Id = @Id
      AND IsDeleted = 0;

    SELECT @Id AS GuId, 0 AS ErrorCode, 'SUCCESS' AS ErrorMessage;
END
GO
