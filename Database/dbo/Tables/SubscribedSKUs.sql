CREATE TABLE [dbo].[SubscribedSKUs] (
    [Id]                 NVARCHAR (255)   NOT NULL,
    [CustomerId]         UNIQUEIDENTIFIER NOT NULL,
    [SkuBusinessId]      UNIQUEIDENTIFIER NOT NULL,
    [PartNumber]         NVARCHAR (255)   NOT NULL,
    [OfferName]          NVARCHAR (255)   NULL,
    [CapabilityStatus]   NCHAR (10)       NOT NULL,
    [ActiveSeats]        INT              NOT NULL,
    [InGracePeriodSeats] INT              NOT NULL,
    [DisabledSeats]      INT              NOT NULL,
    [AssignedSeats]      INT              NOT NULL,
    CONSTRAINT [PK_SubscribedSKUs] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_SubscribedSKUs_Customers] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers] ([Id])
);














GO
CREATE NONCLUSTERED INDEX [IX_SubscribedSKUs_CustomerId]
    ON [dbo].[SubscribedSKUs]([CustomerId] ASC);


GO




CREATE TRIGGER [dbo].[SubscribedSKUs_Update_Trigger] 
ON [dbo].[SubscribedSKUs]
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
	INSERT INTO SubscribedSKUs_History SELECT @CorrelationId,* FROM inserted
GO




CREATE TRIGGER [dbo].[SubscribedSKUs_Insert_Trigger] 
ON [dbo].[SubscribedSKUs]
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
	INSERT INTO SubscribedSKUs_History SELECT @CorrelationId,* FROM inserted