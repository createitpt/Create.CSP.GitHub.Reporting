CREATE TABLE [dbo].[MarketingCampaigns] (
    [Id]        UNIQUEIDENTIFIER CONSTRAINT [DF_MarketingCampaigns_Id] DEFAULT (newid()) NOT NULL,
    [Name]      NVARCHAR (255)   NOT NULL,
    [StartDate] DATE             NOT NULL,
    [EndDate]   DATE             NOT NULL,
    CONSTRAINT [PK_MarketingCampaigns] PRIMARY KEY CLUSTERED ([Id] ASC)
);



