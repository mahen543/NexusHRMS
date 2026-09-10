CREATE OR ALTER PROCEDURE dbo.usp_DeleteEmployee
(
    @Id NVARCHAR(64),
    @DeletedBy NVARCHAR(128),
    @IPAddress NVARCHAR(64) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    UPDATE dbo.tblEmployees
    SET
        IsDeleted = 1,
        DeletedBy = @DeletedBy,
        DateDeleted = GETDATE(),
        IPAddress = ISNULL(@IPAddress, IPAddress),
        UpdatedBy = @DeletedBy,
        LastUpdated = GETDATE()
    WHERE Id = @Id
      AND IsDeleted = 0;

    IF @@ROWCOUNT > 0
    BEGIN
        SELECT @Id AS GuId, 0 AS ErrorCode,
               'Employee deleted successfully.' AS ErrorMessage;
    END
    ELSE
    BEGIN
        SELECT @Id AS GuId, 404 AS ErrorCode,
               'Employee record not found or already deleted.' AS ErrorMessage;
    END
END
GO
