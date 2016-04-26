/****** Object:  Table [dbo].[ActivationReport]    Script Date: 26/04/2016 15:42:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ActivationReport](
    [Id] [uniqueidentifier] NOT NULL CONSTRAINT [DF_ActivationReport_Id]  DEFAULT (newid()),
    [CorrelationId] [uniqueidentifier] NOT NULL,
    [CustomerId] [uniqueidentifier] NOT NULL,
    [SubscribedSKUId] [nvarchar](255) NULL,
    [ActionType] [nvarchar](255) NOT NULL,
    [ActionSubType] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_ActivationReport] PRIMARY KEY CLUSTERED 
(
    [Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO
/****** Object:  Table [dbo].[CorrelationIds]    Script Date: 26/04/2016 15:42:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CorrelationIds](
    [Id] [uniqueidentifier] NOT NULL,
    [StartDateTime] [datetime] NOT NULL,
    [EndDateTime] [datetime] NULL,
    [Status] [nvarchar](255) NOT NULL CONSTRAINT [DF_CorrelationIds_Status]  DEFAULT (N'RUNNING'),
 CONSTRAINT [PK_CorrelationIds] PRIMARY KEY CLUSTERED 
(
    [Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO
/****** Object:  Table [dbo].[Customers]    Script Date: 26/04/2016 15:42:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Customers](
    [Id] [uniqueidentifier] NOT NULL,
    [TenantId] [uniqueidentifier] NOT NULL,
    [CompanyName] [nvarchar](255) NOT NULL,
    [AddressLine1] [nvarchar](500) NULL,
    [AddressLine2] [nvarchar](500) NULL,
    [City] [nvarchar](255) NULL,
    [Country] [nvarchar](255) NULL,
    [PostalCode] [nvarchar](255) NULL,
    [Region] [nvarchar](255) NULL,
    [State] [nvarchar](255) NULL,
 CONSTRAINT [PK_Customers] PRIMARY KEY CLUSTERED 
(
    [Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO
/****** Object:  Table [dbo].[Customers_History]    Script Date: 26/04/2016 15:42:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Customers_History](
    [CorrelationId] [uniqueidentifier] NOT NULL,
    [Id] [uniqueidentifier] NOT NULL,
    [TenantId] [uniqueidentifier] NOT NULL,
    [CompanyName] [nvarchar](255) NOT NULL,
    [AddressLine1] [nvarchar](500) NULL,
    [AddressLine2] [nvarchar](500) NULL,
    [City] [nvarchar](255) NULL,
    [Country] [nvarchar](255) NULL,
    [PostalCode] [nvarchar](255) NULL,
    [Region] [nvarchar](255) NULL,
    [State] [nvarchar](255) NULL,
 CONSTRAINT [PK_Customers_History] PRIMARY KEY CLUSTERED 
(
    [CorrelationId] ASC,
    [Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO
/****** Object:  Table [dbo].[Domains]    Script Date: 26/04/2016 15:42:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Domains](
    [Id] [nvarchar](255) NOT NULL CONSTRAINT [DF_Domains_Id]  DEFAULT (newid()),
    [Name] [nvarchar](255) NOT NULL,
    [CustomerId] [uniqueidentifier] NOT NULL,
    [IsDefault] [bit] NOT NULL,
 CONSTRAINT [PK_Domains] PRIMARY KEY CLUSTERED 
(
    [Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO
/****** Object:  Table [dbo].[Domains_History]    Script Date: 26/04/2016 15:42:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Domains_History](
    [CorrelationId] [uniqueidentifier] NOT NULL,
    [Id] [nvarchar](255) NOT NULL,
    [Name] [nvarchar](255) NOT NULL,
    [CustomerId] [uniqueidentifier] NOT NULL,
    [IsDefault] [bit] NOT NULL,
 CONSTRAINT [PK_Domains_History] PRIMARY KEY CLUSTERED 
(
    [CorrelationId] ASC,
    [Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO
/****** Object:  Table [dbo].[Log]    Script Date: 26/04/2016 15:42:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Log](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [Date] [datetime] NOT NULL,
    [Thread] [nvarchar](255) NOT NULL,
    [Level] [nvarchar](50) NOT NULL,
    [Logger] [nvarchar](255) NOT NULL,
    [Message] [nvarchar](max) NOT NULL,
    [Exception] [nvarchar](max) NULL
)

GO
/****** Object:  Table [dbo].[MarketingCampaigns]    Script Date: 26/04/2016 15:42:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MarketingCampaigns](
    [Id] [uniqueidentifier] NOT NULL CONSTRAINT [DF_MarketingCampaigns_Id]  DEFAULT (newid()),
    [Name] [nvarchar](255) NOT NULL,
    [StartDate] [date] NOT NULL,
    [EndDate] [date] NOT NULL,
 CONSTRAINT [PK_MarketingCampaigns] PRIMARY KEY CLUSTERED 
(
    [Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO
/****** Object:  Table [dbo].[MarketingCampaignsCustomers]    Script Date: 26/04/2016 15:42:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MarketingCampaignsCustomers](
    [Id] [uniqueidentifier] NOT NULL CONSTRAINT [DF_MarketingCampaignsCustomers_Id]  DEFAULT (newid()),
    [MarketingCampaignId] [uniqueidentifier] NOT NULL,
    [CustomerId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_MarketingCampaignsCustomers] PRIMARY KEY CLUSTERED 
(
    [Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF),
 CONSTRAINT [IX_MarketingCampaignsCustomers_CustomerId_MarketingCampaign] UNIQUE NONCLUSTERED 
(
    [CustomerId] ASC,
    [MarketingCampaignId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO
/****** Object:  Table [dbo].[SubscribedSKUs]    Script Date: 26/04/2016 15:42:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SubscribedSKUs](
    [Id] [nvarchar](255) NOT NULL,
    [CustomerId] [uniqueidentifier] NOT NULL,
    [SkuBusinessId] [uniqueidentifier] NOT NULL,
    [PartNumber] [nvarchar](255) NOT NULL,
    [OfferName] [nvarchar](255) NULL,
    [CapabilityStatus] [nchar](10) NOT NULL,
    [ActiveSeats] [int] NOT NULL,
    [InGracePeriodSeats] [int] NOT NULL,
    [DisabledSeats] [int] NOT NULL,
    [AssignedSeats] [int] NOT NULL,
 CONSTRAINT [PK_SubscribedSKUs] PRIMARY KEY CLUSTERED 
(
    [Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO
/****** Object:  Table [dbo].[SubscribedSKUs_History]    Script Date: 26/04/2016 15:42:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SubscribedSKUs_History](
    [CorrelationId] [uniqueidentifier] NOT NULL,
    [Id] [nvarchar](255) NOT NULL,
    [CustomerId] [uniqueidentifier] NOT NULL,
    [SkuBusinessId] [uniqueidentifier] NOT NULL,
    [PartNumber] [nvarchar](255) NOT NULL,
    [OfferName] [nvarchar](255) NULL,
    [CapabilityStatus] [nchar](10) NOT NULL,
    [ActiveSeats] [int] NOT NULL,
    [InGracePeriodSeats] [int] NOT NULL,
    [DisabledSeats] [int] NOT NULL,
    [AssignedSeats] [int] NOT NULL,
 CONSTRAINT [PK_SubscribedSKUs_History] PRIMARY KEY CLUSTERED 
(
    [CorrelationId] ASC,
    [Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO
/****** Object:  Table [dbo].[Subscriptions]    Script Date: 26/04/2016 15:42:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Subscriptions](
    [Id] [uniqueidentifier] NOT NULL,
    [CustomerId] [uniqueidentifier] NOT NULL,
    [SubscribedSKUId] [nvarchar](255) NOT NULL,
    [Status] [nchar](10) NOT NULL,
    [Quantity] [int] NOT NULL,
    [CreationDate] [datetime] NOT NULL,
 CONSTRAINT [PK_Subscriptions] PRIMARY KEY CLUSTERED 
(
    [Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO
/****** Object:  Table [dbo].[Subscriptions_History]    Script Date: 26/04/2016 15:42:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Subscriptions_History](
    [CorrelationId] [uniqueidentifier] NOT NULL,
    [Id] [uniqueidentifier] NOT NULL,
    [CustomerId] [uniqueidentifier] NOT NULL,
    [SubscribedSKUId] [nvarchar](255) NOT NULL,
    [Status] [nchar](10) NOT NULL,
    [Quantity] [int] NOT NULL,
    [CreationDate] [datetime] NOT NULL,
 CONSTRAINT [PK_Subscriptions_History] PRIMARY KEY CLUSTERED 
(
    [CorrelationId] ASC,
    [Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO
/****** Object:  Index [IX_ActivationReport_CustomerId]    Script Date: 26/04/2016 15:42:20 ******/
CREATE NONCLUSTERED INDEX [IX_ActivationReport_CustomerId] ON [dbo].[ActivationReport]
(
    [CustomerId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_Domains_CustomerId]    Script Date: 26/04/2016 15:42:20 ******/
CREATE NONCLUSTERED INDEX [IX_Domains_CustomerId] ON [dbo].[Domains]
(
    [CustomerId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_Domains_History_CustomerId]    Script Date: 26/04/2016 15:42:20 ******/
CREATE NONCLUSTERED INDEX [IX_Domains_History_CustomerId] ON [dbo].[Domains_History]
(
    [CustomerId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_SubscribedSKUs_CustomerId]    Script Date: 26/04/2016 15:42:20 ******/
CREATE NONCLUSTERED INDEX [IX_SubscribedSKUs_CustomerId] ON [dbo].[SubscribedSKUs]
(
    [CustomerId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_SubscribedSKUs_History_CustomerId]    Script Date: 26/04/2016 15:42:20 ******/
CREATE NONCLUSTERED INDEX [IX_SubscribedSKUs_History_CustomerId] ON [dbo].[SubscribedSKUs_History]
(
    [CustomerId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_Subscriptions_CustomerId]    Script Date: 26/04/2016 15:42:20 ******/
CREATE NONCLUSTERED INDEX [IX_Subscriptions_CustomerId] ON [dbo].[Subscriptions]
(
    [CustomerId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_Subscriptions_History_CustomerId]    Script Date: 26/04/2016 15:42:20 ******/
CREATE NONCLUSTERED INDEX [IX_Subscriptions_History_CustomerId] ON [dbo].[Subscriptions_History]
(
    [CustomerId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
ALTER TABLE [dbo].[ActivationReport]  WITH CHECK ADD  CONSTRAINT [FK_ActivationReport_CorrelationIds] FOREIGN KEY([CorrelationId])
REFERENCES [dbo].[CorrelationIds] ([Id])
GO
ALTER TABLE [dbo].[ActivationReport] CHECK CONSTRAINT [FK_ActivationReport_CorrelationIds]
GO
ALTER TABLE [dbo].[ActivationReport]  WITH CHECK ADD  CONSTRAINT [FK_ActivationReport_Customers] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([Id])
GO
ALTER TABLE [dbo].[ActivationReport] CHECK CONSTRAINT [FK_ActivationReport_Customers]
GO
ALTER TABLE [dbo].[ActivationReport]  WITH CHECK ADD  CONSTRAINT [FK_ActivationReport_SubscribedSKUs] FOREIGN KEY([SubscribedSKUId])
REFERENCES [dbo].[SubscribedSKUs] ([Id])
GO
ALTER TABLE [dbo].[ActivationReport] CHECK CONSTRAINT [FK_ActivationReport_SubscribedSKUs]
GO
ALTER TABLE [dbo].[Customers_History]  WITH CHECK ADD  CONSTRAINT [FK_Customers_History_CorrelationIds] FOREIGN KEY([CorrelationId])
REFERENCES [dbo].[CorrelationIds] ([Id])
GO
ALTER TABLE [dbo].[Customers_History] CHECK CONSTRAINT [FK_Customers_History_CorrelationIds]
GO
ALTER TABLE [dbo].[Domains]  WITH CHECK ADD  CONSTRAINT [FK_Domains_Customers] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([Id])
GO
ALTER TABLE [dbo].[Domains] CHECK CONSTRAINT [FK_Domains_Customers]
GO
ALTER TABLE [dbo].[Domains_History]  WITH CHECK ADD  CONSTRAINT [FK_Domains_History_CorrelationIds] FOREIGN KEY([CorrelationId])
REFERENCES [dbo].[CorrelationIds] ([Id])
GO
ALTER TABLE [dbo].[Domains_History] CHECK CONSTRAINT [FK_Domains_History_CorrelationIds]
GO
ALTER TABLE [dbo].[Domains_History]  WITH CHECK ADD  CONSTRAINT [FK_Domains_History_Customers_History] FOREIGN KEY([CorrelationId], [CustomerId])
REFERENCES [dbo].[Customers_History] ([CorrelationId], [Id])
GO
ALTER TABLE [dbo].[Domains_History] CHECK CONSTRAINT [FK_Domains_History_Customers_History]
GO
ALTER TABLE [dbo].[MarketingCampaignsCustomers]  WITH CHECK ADD  CONSTRAINT [FK_MarketingCampaignsCustomers_Customers] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([Id])
GO
ALTER TABLE [dbo].[MarketingCampaignsCustomers] CHECK CONSTRAINT [FK_MarketingCampaignsCustomers_Customers]
GO
ALTER TABLE [dbo].[MarketingCampaignsCustomers]  WITH CHECK ADD  CONSTRAINT [FK_MarketingCampaignsCustomers_MarketingCampaigns] FOREIGN KEY([MarketingCampaignId])
REFERENCES [dbo].[MarketingCampaigns] ([Id])
GO
ALTER TABLE [dbo].[MarketingCampaignsCustomers] CHECK CONSTRAINT [FK_MarketingCampaignsCustomers_MarketingCampaigns]
GO
ALTER TABLE [dbo].[SubscribedSKUs]  WITH CHECK ADD  CONSTRAINT [FK_SubscribedSKUs_Customers] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([Id])
GO
ALTER TABLE [dbo].[SubscribedSKUs] CHECK CONSTRAINT [FK_SubscribedSKUs_Customers]
GO
ALTER TABLE [dbo].[SubscribedSKUs_History]  WITH CHECK ADD  CONSTRAINT [FK_SubscribedSKUs_History_CorrelationIds] FOREIGN KEY([CorrelationId])
REFERENCES [dbo].[CorrelationIds] ([Id])
GO
ALTER TABLE [dbo].[SubscribedSKUs_History] CHECK CONSTRAINT [FK_SubscribedSKUs_History_CorrelationIds]
GO
ALTER TABLE [dbo].[SubscribedSKUs_History]  WITH CHECK ADD  CONSTRAINT [FK_SubscribedSKUs_History_Customers_History] FOREIGN KEY([CorrelationId], [CustomerId])
REFERENCES [dbo].[Customers_History] ([CorrelationId], [Id])
GO
ALTER TABLE [dbo].[SubscribedSKUs_History] CHECK CONSTRAINT [FK_SubscribedSKUs_History_Customers_History]
GO
ALTER TABLE [dbo].[Subscriptions]  WITH CHECK ADD  CONSTRAINT [FK_Subscriptions_Customers] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([Id])
GO
ALTER TABLE [dbo].[Subscriptions] CHECK CONSTRAINT [FK_Subscriptions_Customers]
GO
ALTER TABLE [dbo].[Subscriptions]  WITH CHECK ADD  CONSTRAINT [FK_Subscriptions_SubscribedSKUs] FOREIGN KEY([SubscribedSKUId])
REFERENCES [dbo].[SubscribedSKUs] ([Id])
GO
ALTER TABLE [dbo].[Subscriptions] CHECK CONSTRAINT [FK_Subscriptions_SubscribedSKUs]
GO
ALTER TABLE [dbo].[Subscriptions_History]  WITH CHECK ADD  CONSTRAINT [FK_Subscriptions_History_CorrelationIds] FOREIGN KEY([CorrelationId])
REFERENCES [dbo].[CorrelationIds] ([Id])
GO
ALTER TABLE [dbo].[Subscriptions_History] CHECK CONSTRAINT [FK_Subscriptions_History_CorrelationIds]
GO
ALTER TABLE [dbo].[Subscriptions_History]  WITH CHECK ADD  CONSTRAINT [FK_Subscriptions_History_Customers_History] FOREIGN KEY([CorrelationId], [CustomerId])
REFERENCES [dbo].[Customers_History] ([CorrelationId], [Id])
GO
ALTER TABLE [dbo].[Subscriptions_History] CHECK CONSTRAINT [FK_Subscriptions_History_Customers_History]
GO
ALTER TABLE [dbo].[Subscriptions_History]  WITH CHECK ADD  CONSTRAINT [FK_Subscriptions_History_SubscribedSKUs_History] FOREIGN KEY([CorrelationId], [SubscribedSKUId])
REFERENCES [dbo].[SubscribedSKUs_History] ([CorrelationId], [Id])
GO
ALTER TABLE [dbo].[Subscriptions_History] CHECK CONSTRAINT [FK_Subscriptions_History_SubscribedSKUs_History]
GO
/****** Object:  StoredProcedure [dbo].[GetCorrelationIdFromContext]    Script Date: 26/04/2016 15:42:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetCorrelationIdFromContext]
    @CorrelationId NVARCHAR(36) OUTPUT
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    -- Insert statements for procedure here
    IF SERVERPROPERTY('edition') = 'SQL Azure'
        BEGIN
            -- Use Session Context
            DECLARE @CorrelationIdOut AS NVARCHAR(36)
            EXEC sp_executesql @statement=N'SELECT @CorrelationIdIn = (SELECT CAST(SESSION_CONTEXT(N''CorrelationId'') AS NVARCHAR(36)))',
                @params=N'@CorrelationIdIn AS NVARCHAR(36) OUTPUT',@CorrelationIdIn=@CorrelationIdOut OUTPUT
            SELECT @CorrelationId = @CorrelationIdOut
        END
    ELSE
        BEGIN
            -- Use Context Info
            SELECT @CorrelationId = CONVERT(nvarchar(36),CONVERT(varbinary(128),CONTEXT_INFO()))
        END
    END
GO
/****** Object:  StoredProcedure [dbo].[SetCorrelationIdToContext]    Script Date: 26/04/2016 15:42:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SetCorrelationIdToContext]
    @CorrelationId NVARCHAR(36)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    -- Insert statements for procedure here
    IF SERVERPROPERTY('edition') = 'SQL Azure'
        BEGIN
            -- Use Session Context
            EXEC sp_set_session_context N'CorrelationId', @CorrelationId;
        END
    ELSE
        BEGIN
            -- Use Context Info
            DECLARE @b varbinary(128)
            SET @b = CONVERT(varbinary(128),@CorrelationId)
            EXEC sp_executesql @statement=N'SET CONTEXT_INFO @b',@params=N'@b varbinary(128)',@b=@b
        END
END
GO
/****** Object:  Trigger [dbo].[Customers_Insert_Trigger]    Script Date: 26/04/2016 15:42:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER [dbo].[Customers_Insert_Trigger] 
ON [dbo].[Customers]
FOR INSERT
AS
    -- Get trigger associated table name
    --DECLARE @TableName SYSNAME
    --SELECT @TableName = object_schema_name(parent_id) + '.' + object_name(parent_id) 
    --FROM sys.triggers where object_id = @@PROCID

    -- Get correlation Id from context
    DECLARE @CorrelationId nvarchar(36)
    EXECUTE GetCorrelationIdFromContext @CorrelationId OUTPUT

    IF (@CorrelationId IS NULL)
        BEGIN
            --RAISERROR(N'Correlation Id is NULL', 10, 1)
            THROW 60000, N'Correlation Id is NULL', 1
        END

    -- Insert into history
    INSERT INTO Customers_History SELECT @CorrelationId,* FROM inserted
GO
/****** Object:  Trigger [dbo].[Customers_Update_Trigger]    Script Date: 26/04/2016 15:42:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE TRIGGER [dbo].[Customers_Update_Trigger] 
ON [dbo].[Customers]
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
    INSERT INTO Customers_History SELECT @CorrelationId,* FROM inserted
GO
/****** Object:  Trigger [dbo].[Domains_Insert_Trigger]    Script Date: 26/04/2016 15:42:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE TRIGGER [dbo].[Domains_Insert_Trigger] 
ON [dbo].[Domains]
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
    INSERT INTO Domains_History SELECT @CorrelationId,* FROM inserted
GO
/****** Object:  Trigger [dbo].[Domains_Update_Trigger]    Script Date: 26/04/2016 15:42:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE TRIGGER [dbo].[Domains_Update_Trigger] 
ON [dbo].[Domains]
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
    INSERT INTO Domains_History SELECT @CorrelationId,* FROM inserted
GO
/****** Object:  Trigger [dbo].[SubscribedSKUs_Insert_Trigger]    Script Date: 26/04/2016 15:42:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE TRIGGER [dbo].[SubscribedSKUs_Insert_Trigger] 
ON [dbo].[SubscribedSKUs]
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
    INSERT INTO SubscribedSKUs_History SELECT @CorrelationId,* FROM inserted
GO
/****** Object:  Trigger [dbo].[SubscribedSKUs_Update_Trigger]    Script Date: 26/04/2016 15:42:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE TRIGGER [dbo].[SubscribedSKUs_Update_Trigger] 
ON [dbo].[SubscribedSKUs]
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
    INSERT INTO SubscribedSKUs_History SELECT @CorrelationId,* FROM inserted
GO
/****** Object:  Trigger [dbo].[Subscriptions_Insert_Trigger]    Script Date: 26/04/2016 15:42:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
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
GO
/****** Object:  Trigger [dbo].[Subscriptions_Update_Trigger]    Script Date: 26/04/2016 15:42:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
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
