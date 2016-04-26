CREATE TABLE [dbo].[Customers_History] (
    [CorrelationId] UNIQUEIDENTIFIER NOT NULL,
    [Id]            UNIQUEIDENTIFIER NOT NULL,
    [TenantId]      UNIQUEIDENTIFIER NOT NULL,
    [CompanyName]   NVARCHAR (255)   NOT NULL,
    [AddressLine1]  NVARCHAR (500)   NULL,
    [AddressLine2]  NVARCHAR (500)   NULL,
    [City]          NVARCHAR (255)   NULL,
    [Country]       NVARCHAR (255)   NULL,
    [PostalCode]    NVARCHAR (255)   NULL,
    [Region]        NVARCHAR (255)   NULL,
    [State]         NVARCHAR (255)   NULL,
    CONSTRAINT [PK_Customers_History] PRIMARY KEY CLUSTERED ([CorrelationId] ASC, [Id] ASC),
    CONSTRAINT [FK_Customers_History_CorrelationIds] FOREIGN KEY ([CorrelationId]) REFERENCES [dbo].[CorrelationIds] ([Id])
);







