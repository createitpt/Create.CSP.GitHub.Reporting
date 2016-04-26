CREATE TABLE [dbo].[Customers] (
    [Id]           UNIQUEIDENTIFIER NOT NULL,
    [TenantId]     UNIQUEIDENTIFIER NOT NULL,
    [CompanyName]  NVARCHAR (255)   NOT NULL,
    [AddressLine1] NVARCHAR (500)   NULL,
    [AddressLine2] NVARCHAR (500)   NULL,
    [City]         NVARCHAR (255)   NULL,
    [Country]      NVARCHAR (255)   NULL,
    [PostalCode]   NVARCHAR (255)   NULL,
    [Region]       NVARCHAR (255)   NULL,
    [State]        NVARCHAR (255)   NULL,
    CONSTRAINT [PK_Customers] PRIMARY KEY CLUSTERED ([Id] ASC)
);














GO

CREATE TRIGGER [dbo].[Customers_Insert_Trigger] 
ON [dbo].[Customers]
FOR INSERT
AS
	-- Get trigger associated table name
	--DECLARE @TableName SYSNAME
	--SELECT @TableName = object_schema_name(parent_id) + '.' + object_name(parent_id) 
	--FROM sys.triggers where object_id = @@PROCID

	-- Get correlation Id from context
	DECLARE @CorrelationId nvarchar(36)
	EXECUTE GetCorrelationIdFromContext @CorrelationId OUTPUT

	IF (@CorrelationId IS NULL)
		BEGIN
			--RAISERROR(N'Correlation Id is NULL', 10, 1)
			THROW 60000, N'Correlation Id is NULL', 1
		END

	-- Insert into history
	INSERT INTO Customers_History SELECT @CorrelationId,* FROM inserted
GO


GO



CREATE TRIGGER [dbo].[Customers_Update_Trigger] 
ON [dbo].[Customers]
FOR UPDATE
AS

	-- Get correlation Id from context
	DECLARE @CorrelationId nvarchar(36)
	EXECUTE GetCorrelationIdFromContext @CorrelationId OUTPUT

	IF (@CorrelationId IS NULL)
		BEGIN
			--RAISERROR(N'Correlation Id is NULL', 10, 1)
			THROW 60000, N'Correlation Id is NULL', 1
		END

	-- Insert into history
	INSERT INTO Customers_History SELECT @CorrelationId,* FROM inserted