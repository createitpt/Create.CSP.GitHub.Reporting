
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetCorrelationIdFromContext]
	@CorrelationId NVARCHAR(36) OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	IF SERVERPROPERTY('edition') = 'SQL Azure'
		BEGIN
			-- Use Session Context
			DECLARE @CorrelationIdOut AS NVARCHAR(36)
			EXEC sp_executesql @statement=N'SELECT @CorrelationIdIn = (SELECT CAST(SESSION_CONTEXT(N''CorrelationId'') AS NVARCHAR(36)))',
				@params=N'@CorrelationIdIn AS NVARCHAR(36) OUTPUT',@CorrelationIdIn=@CorrelationIdOut OUTPUT
			SELECT @CorrelationId = @CorrelationIdOut
		END
	ELSE
		BEGIN
			-- Use Context Info
			SELECT @CorrelationId = CONVERT(nvarchar(36),CONVERT(varbinary(128),CONTEXT_INFO()))
		END
	END