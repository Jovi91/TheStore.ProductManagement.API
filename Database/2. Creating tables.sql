/*
Run this script on db:

        (local)\SQLEXPRESS.THESTORE   

*/
SET NUMERIC_ROUNDABORT OFF
GO
SET ANSI_PADDING, ANSI_WARNINGS, CONCAT_NULL_YIELDS_NULL, ARITHABORT, QUOTED_IDENTIFIER, ANSI_NULLS ON
GO
SET XACT_ABORT ON
GO
SET TRANSACTION ISOLATION LEVEL Serializable
GO
BEGIN TRANSACTION
GO

IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating schemas'
GO
CREATE SCHEMA [api]
AUTHORIZATION [dbo]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
CREATE SCHEMA [logs]
AUTHORIZATION [dbo]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating [dbo].[Product]'
GO
CREATE TABLE [dbo].[Product]
(
[ProductId] [int] NOT NULL IDENTITY(1, 1),
[ProductName] [nvarchar] (50) COLLATE Polish_CI_AS NOT NULL,
[ProductDescription] [nvarchar] (255) COLLATE Polish_CI_AS NULL,
[ProductBrand] [nvarchar] (50) COLLATE Polish_CI_AS NOT NULL,
[Sysdat] [datetime] NOT NULL CONSTRAINT [DF_Product_Sysdat] DEFAULT (getdate()),
[Sysuser] [nvarchar] (50) COLLATE Polish_CI_AS NOT NULL CONSTRAINT [DF_Product_Sysuser] DEFAULT (suser_sname())
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating primary key [PK_Product] on [dbo].[Product]'
GO
ALTER TABLE [dbo].[Product] ADD CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED ([ProductId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating index [IX_Product_ProductName] on [dbo].[Product]'
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Product_ProductName] ON [dbo].[Product] ([ProductName])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating [dbo].[Price]'
GO
CREATE TABLE [dbo].[Price]
(
[PriceId] [int] NOT NULL IDENTITY(1, 1),
[ProductId] [int] NOT NULL,
[Currency] [char] (3) COLLATE Polish_CI_AS NOT NULL,
[Price] [decimal] (10, 2) NOT NULL,
[StartDate] [date] NOT NULL,
[EndDate] [date] NOT NULL,
[Sysdat] [datetime] NOT NULL CONSTRAINT [DF_Price_Sysdat] DEFAULT (getdate()),
[Sysuser] [nvarchar] (50) COLLATE Polish_CI_AS NOT NULL CONSTRAINT [DF_Price_Sysuser] DEFAULT (suser_sname())
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating primary key [PK_Price] on [dbo].[Price]'
GO
ALTER TABLE [dbo].[Price] ADD CONSTRAINT [PK_Price] PRIMARY KEY CLUSTERED ([PriceId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating [logs].[ProcedureExecution]'
GO
CREATE TABLE [logs].[ProcedureExecution]
(
[ProcedureExecutionId] [int] NOT NULL IDENTITY(1, 1),
[ProcedureName] [nvarchar] (255) COLLATE Polish_CI_AS NULL,
[Parameters] [nvarchar] (max) COLLATE Polish_CI_AS NULL,
[ExecutionStatus] [nvarchar] (50) COLLATE Polish_CI_AS NULL,
[ErrorMessage] [nvarchar] (max) COLLATE Polish_CI_AS NULL,
[Duration] [int] NULL,
[OutputParams] [nvarchar] (max) COLLATE Polish_CI_AS NULL,
[SysDat] [datetime] NOT NULL CONSTRAINT [DF_ProcedureExecution_SysDat] DEFAULT (getdate()),
[SysUser] [nvarchar] (50) COLLATE Polish_CI_AS NOT NULL CONSTRAINT [DF_ProcedureExecution_SysUser] DEFAULT (suser_sname())
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating primary key [PK__Procedur__55D4A14763FD1303] on [logs].[ProcedureExecution]'
GO
ALTER TABLE [logs].[ProcedureExecution] ADD CONSTRAINT [PK__Procedur__55D4A14763FD1303] PRIMARY KEY CLUSTERED ([ProcedureExecutionId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating [dbo].[ProductApiErrorLogs]'
GO
CREATE TABLE [dbo].[ProductApiErrorLogs]
(
[ApiErrorLogId] [int] NOT NULL IDENTITY(1, 1),
[IpAddress] [nvarchar] (50) COLLATE Polish_CI_AS NULL,
[Endpoint] [nvarchar] (255) COLLATE Polish_CI_AS NULL,
[RequestTimestamp] [datetime] NULL,
[RequestMethod] [nvarchar] (10) COLLATE Polish_CI_AS NULL,
[StatusCode] [int] NULL,
[ErrorMessage] [nvarchar] (max) COLLATE Polish_CI_AS NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating primary key [PK__ProductA__3285FC54C8DE2099] on [dbo].[ProductApiErrorLogs]'
GO
ALTER TABLE [dbo].[ProductApiErrorLogs] ADD CONSTRAINT [PK__ProductA__3285FC54C8DE2099] PRIMARY KEY CLUSTERED ([ApiErrorLogId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Adding foreign keys to [dbo].[Price]'
GO
ALTER TABLE [dbo].[Price] ADD CONSTRAINT [FK_Price_Product] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Product] ([ProductId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
COMMIT TRANSACTION
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
-- This statement writes to the SQL Server Log so SQL Monitor can show this deployment.
IF HAS_PERMS_BY_NAME(N'sys.xp_logevent', N'OBJECT', N'EXECUTE') = 1
BEGIN
    DECLARE @databaseName AS nvarchar(2048), @eventMessage AS nvarchar(2048)
    SET @databaseName = REPLACE(REPLACE(DB_NAME(), N'\', N'\\'), N'"', N'\"')
    SET @eventMessage = N'Redgate SQL Compare: { "deployment": { "description": "Redgate SQL Compare deployed to ' + @databaseName + N'", "database": "' + @databaseName + N'" }}'
    EXECUTE sys.xp_logevent 55000, @eventMessage
END
GO
DECLARE @Success AS BIT
SET @Success = 1
SET NOEXEC OFF
IF (@Success = 1) PRINT 'The database update succeeded'
ELSE BEGIN
	IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
	PRINT 'The database update failed'
END
GO
