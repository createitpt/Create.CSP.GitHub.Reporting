CREATE TABLE [dbo].[MarketingCampaignsCustomers] (
    [Id]                  UNIQUEIDENTIFIER CONSTRAINT [DF_MarketingCampaignsCustomers_Id] DEFAULT (newid()) NOT NULL,
    [MarketingCampaignId] UNIQUEIDENTIFIER NOT NULL,
    [CustomerId]          UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_MarketingCampaignsCustomers] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_MarketingCampaignsCustomers_Customers] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers] ([Id]),
    CONSTRAINT [FK_MarketingCampaignsCustomers_MarketingCampaigns] FOREIGN KEY ([MarketingCampaignId]) REFERENCES [dbo].[MarketingCampaigns] ([Id]),
    CONSTRAINT [IX_MarketingCampaignsCustomers_CustomerId_MarketingCampaign] UNIQUE NONCLUSTERED ([CustomerId] ASC, [MarketingCampaignId] ASC)
);







