CREATE OR ALTER PROCEDURE dbo.usp_GetErrorMessage
(
    @ErrorCode INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT ErrorCode, ErrorName, ErrorMessage
    FROM dbo.tblErrorCode
    WHERE ErrorCode = @ErrorCode
      AND IsDeleted = 0;
END
GO
