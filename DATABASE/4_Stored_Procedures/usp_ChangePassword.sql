CREATE OR ALTER PROCEDURE dbo.usp_ChangePassword
(
    @UserId BIGINT,
    @NewPassword NVARCHAR(512),
    @IPAddress NVARCHAR(64) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    UPDATE dbo.tblUsers
    SET
        PasswordHash = @NewPassword,
        IPAddress = ISNULL(@IPAddress, IPAddress)
    WHERE UserId = @UserId
      AND IsActive = 1
      AND IsDeleted = 0;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO
