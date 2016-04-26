CREATE TABLE [dbo].[Domains_History] (
    [CorrelationId] UNIQUEIDENTIFIER NOT NULL,
    [Id]            NVARCHAR (255)   NOT NULL,
    [Name]          NVARCHAR (255)   NOT NULL,
    [CustomerId]    UNIQUEIDENTIFIER NOT NULL,
    [IsDefault]     BIT              NOT NULL,
    CONSTRAINT [PK_Domains_History] PRIMARY KEY CLUSTERED ([CorrelationId] ASC, [Id] ASC),
    CONSTRAINT [FK_Domains_History_CorrelationIds] FOREIGN KEY ([CorrelationId]) REFERENCES [dbo].[CorrelationIds] ([Id]),
    CONSTRAINT [FK_Domains_History_Customers_History] FOREIGN KEY ([CorrelationId], [CustomerId]) REFERENCES [dbo].[Customers_History] ([CorrelationId], [Id])
);










GO
CREATE NONCLUSTERED INDEX [IX_Domains_History_CustomerId]
    ON [dbo].[Domains_History]([CustomerId] ASC);

