CREATE TABLE [dbo].[ActivationReport] (
    [Id]              UNIQUEIDENTIFIER CONSTRAINT [DF_ActivationReport_Id] DEFAULT (newid()) NOT NULL,
    [CorrelationId]   UNIQUEIDENTIFIER NOT NULL,
    [CustomerId]      UNIQUEIDENTIFIER NOT NULL,
    [SubscribedSKUId] NVARCHAR (255)   NULL,
    [ActionType]      NVARCHAR (255)   NOT NULL,
    [ActionSubType]   NVARCHAR (MAX)   NOT NULL,
    CONSTRAINT [PK_ActivationReport] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ActivationReport_CorrelationIds] FOREIGN KEY ([CorrelationId]) REFERENCES [dbo].[CorrelationIds] ([Id]),
    CONSTRAINT [FK_ActivationReport_Customers] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers] ([Id]),
    CONSTRAINT [FK_ActivationReport_SubscribedSKUs] FOREIGN KEY ([SubscribedSKUId]) REFERENCES [dbo].[SubscribedSKUs] ([Id])
);












GO



GO
CREATE NONCLUSTERED INDEX [IX_ActivationReport_CustomerId]
    ON [dbo].[ActivationReport]([CustomerId] ASC);

