-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE SetCorrelationIdToContext
	@CorrelationId NVARCHAR(36)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	IF SERVERPROPERTY('edition') = 'SQL Azure'
		BEGIN
			-- Use Session Context
			EXEC sp_set_session_context N'CorrelationId', @CorrelationId;
		END
	ELSE
		BEGIN
			-- Use Context Info
			DECLARE @b varbinary(128)
			SET @b = CONVERT(varbinary(128),@CorrelationId)
			EXEC sp_executesql @statement=N'SET CONTEXT_INFO @b',@params=N'@b varbinary(128)',@b=@b
		END
END