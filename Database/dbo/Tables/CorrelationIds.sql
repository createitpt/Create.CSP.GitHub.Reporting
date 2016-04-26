CREATE TABLE [dbo].[CorrelationIds] (
    [Id]            UNIQUEIDENTIFIER NOT NULL,
    [StartDateTime] DATETIME         NOT NULL,
    [EndDateTime]   DATETIME         NULL,
    [Status]        NVARCHAR (255)   CONSTRAINT [DF_CorrelationIds_Status] DEFAULT (N'RUNNING') NOT NULL,
    CONSTRAINT [PK_CorrelationIds] PRIMARY KEY CLUSTERED ([Id] ASC)
);





