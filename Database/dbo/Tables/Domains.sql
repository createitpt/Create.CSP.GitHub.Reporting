CREATE TABLE [dbo].[Domains] (
    [Id]         NVARCHAR (255)   CONSTRAINT [DF_Domains_Id] DEFAULT (newid()) NOT NULL,
    [Name]       NVARCHAR (255)   NOT NULL,
    [CustomerId] UNIQUEIDENTIFIER NOT NULL,
    [IsDefault]  BIT              NOT NULL,
    CONSTRAINT [PK_Domains] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Domains_Customers] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers] ([Id])
);












GO
CREATE NONCLUSTERED INDEX [IX_Domains_CustomerId]
    ON [dbo].[Domains]([CustomerId] ASC);


GO




CREATE TRIGGER [dbo].[Domains_Update_Trigger] 
ON [dbo].[Domains]
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
	INSERT INTO Domains_History SELECT @CorrelationId,* FROM inserted
GO




CREATE TRIGGER [dbo].[Domains_Insert_Trigger] 
ON [dbo].[Domains]
FOR INSERT
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
	INSERT INTO Domains_History SELECT @CorrelationId,* FROM inserted