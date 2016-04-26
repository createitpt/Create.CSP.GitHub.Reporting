CREATE TABLE [dbo].[SubscribedSKUs_History] (
    [CorrelationId]      UNIQUEIDENTIFIER NOT NULL,
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
    CONSTRAINT [PK_SubscribedSKUs_History] PRIMARY KEY CLUSTERED ([CorrelationId] ASC, [Id] ASC),
    CONSTRAINT [FK_SubscribedSKUs_History_CorrelationIds] FOREIGN KEY ([CorrelationId]) REFERENCES [dbo].[CorrelationIds] ([Id]),
    CONSTRAINT [FK_SubscribedSKUs_History_Customers_History] FOREIGN KEY ([CorrelationId], [CustomerId]) REFERENCES [dbo].[Customers_History] ([CorrelationId], [Id])
);










GO
CREATE NONCLUSTERED INDEX [IX_SubscribedSKUs_History_CustomerId]
    ON [dbo].[SubscribedSKUs_History]([CustomerId] ASC);

