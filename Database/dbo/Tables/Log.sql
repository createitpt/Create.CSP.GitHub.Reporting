CREATE TABLE [dbo].[Log] (
    [Id]        INT            IDENTITY (1, 1) NOT NULL,
    [Date]      DATETIME       NOT NULL,
    [Thread]    NVARCHAR (255) NOT NULL,
    [Level]     NVARCHAR (50)  NOT NULL,
    [Logger]    NVARCHAR (255) NOT NULL,
    [Message]   NVARCHAR (MAX) NOT NULL,
    [Exception] NVARCHAR (MAX) NULL
);

