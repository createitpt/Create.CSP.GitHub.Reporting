CREATE TABLE [dbo].[Subscriptions_History] (
    [CorrelationId]   UNIQUEIDENTIFIER NOT NULL,
    [Id]              UNIQUEIDENTIFIER NOT NULL,
    [CustomerId]      UNIQUEIDENTIFIER NOT NULL,
    [SubscribedSKUId] NVARCHAR (255)   NOT NULL,
    [Status]          NCHAR (10)       NOT NULL,
    [Quantity]        INT              NOT NULL,
    [CreationDate]    DATETIME         NOT NULL,
    CONSTRAINT [PK_Subscriptions_History] PRIMARY KEY CLUSTERED ([CorrelationId] ASC, [Id] ASC),
    CONSTRAINT [FK_Subscriptions_History_CorrelationIds] FOREIGN KEY ([CorrelationId]) REFERENCES [dbo].[CorrelationIds] ([Id]),
    CONSTRAINT [FK_Subscriptions_History_Customers_History] FOREIGN KEY ([CorrelationId], [CustomerId]) REFERENCES [dbo].[Customers_History] ([CorrelationId], [Id]),
    CONSTRAINT [FK_Subscriptions_History_SubscribedSKUs_History] FOREIGN KEY ([CorrelationId], [SubscribedSKUId]) REFERENCES [dbo].[SubscribedSKUs_History] ([CorrelationId], [Id])
);










GO
CREATE NONCLUSTERED INDEX [IX_Subscriptions_History_CustomerId]
    ON [dbo].[Subscriptions_History]([CustomerId] ASC);

