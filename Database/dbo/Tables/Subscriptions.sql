CREATE TABLE [dbo].[Subscriptions] (
    [Id]              UNIQUEIDENTIFIER NOT NULL,
    [CustomerId]      UNIQUEIDENTIFIER NOT NULL,
    [SubscribedSKUId] NVARCHAR (255)   NOT NULL,
    [Status]          NCHAR (10)       NOT NULL,
    [Quantity]        INT              NOT NULL,
    [CreationDate]    DATETIME         NOT NULL,
    CONSTRAINT [PK_Subscriptions] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Subscriptions_Customers] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers] ([Id]),
    CONSTRAINT [FK_Subscriptions_SubscribedSKUs] FOREIGN KEY ([SubscribedSKUId]) REFERENCES [dbo].[SubscribedSKUs] ([Id])
);










GO
CREATE NONCLUSTERED INDEX [IX_Subscriptions_CustomerId]
    ON [dbo].[Subscriptions]([CustomerId] ASC);


GO




CREATE TRIGGER [dbo].[Subscriptions_Update_Trigger] 
ON [dbo].[Subscriptions]
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
	INSERT INTO Subscriptions_History SELECT @CorrelationId,* FROM inserted
GO




CREATE TRIGGER [dbo].[Subscriptions_Insert_Trigger] 
ON [dbo].[Subscriptions]
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
	INSERT INTO Subscriptions_History SELECT @CorrelationId,* FROM inserted