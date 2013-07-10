﻿/*
Deployment script for MapperTestDb

This code was generated by a tool.
Changes to this file may cause incorrect behavior and will be lost if
the code is regenerated.
*/

GO
SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;

SET NUMERIC_ROUNDABORT OFF;


GO
:setvar DatabaseName "MapperTestDb"
:setvar DefaultFilePrefix "MapperTestDb"
:setvar DefaultDataPath "C:\Program Files\Microsoft SQL Server\MSSQL10_50.MSSQLSERVER\MSSQL\DATA\"
:setvar DefaultLogPath "C:\Program Files\Microsoft SQL Server\MSSQL10_50.MSSQLSERVER\MSSQL\DATA\"

GO
:on error exit
GO
/*
Detect SQLCMD mode and disable script execution if SQLCMD mode is not supported.
To re-enable the script after enabling SQLCMD mode, execute the following:
SET NOEXEC OFF; 
*/
:setvar __IsSqlCmdEnabled "True"
GO
IF N'$(__IsSqlCmdEnabled)' NOT LIKE N'True'
    BEGIN
        PRINT N'SQLCMD mode must be enabled to successfully execute this script.';
        SET NOEXEC ON;
    END


GO
USE [$(DatabaseName)];


GO
PRINT N'Rename refactoring operation with key 00698261-27d9-4932-985c-cf700b007c32 is skipped, element [dbo].[Client].[Id] (SqlSimpleColumn) will not be renamed to Name';


GO
PRINT N'Rename refactoring operation with key 98d2f3eb-86dc-4b42-a9ff-d479703de2b8 is skipped, element [dbo].[PhoneType].[Id] (SqlSimpleColumn) will not be renamed to Name';


GO
PRINT N'Rename refactoring operation with key fb8c4147-28f9-4a2b-9f50-26a77e80c6c0 is skipped, element [dbo].[Phone].[Id] (SqlSimpleColumn) will not be renamed to AreaCode';


GO
PRINT N'Rename refactoring operation with key e9e5ce87-0fe1-4bbc-a489-56085015797e is skipped, element [dbo].[ClientsPhone].[Id] (SqlSimpleColumn) will not be renamed to ClientName';


GO
PRINT N'Starting rebuilding table [dbo].[Client]...';


GO
BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

CREATE TABLE [dbo].[tmp_ms_xx_Client] (
    [Name] NVARCHAR (255) NOT NULL,
    PRIMARY KEY CLUSTERED ([Name] ASC)
);

IF EXISTS (SELECT TOP 1 1 
           FROM   [dbo].[Client])
    BEGIN
        INSERT INTO [dbo].[tmp_ms_xx_Client] ([Name])
        SELECT   [Name]
        FROM     [dbo].[Client]
        ORDER BY [Name] ASC;
    END

DROP TABLE [dbo].[Client];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_Client]', N'Client';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;


GO
PRINT N'Starting rebuilding table [dbo].[Phone]...';


GO
BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

CREATE TABLE [dbo].[tmp_ms_xx_Phone] (
    [AreaCode]      INT            NOT NULL,
    [Number]        INT            NOT NULL,
    [PhoneTypeName] NVARCHAR (255) NOT NULL,
    PRIMARY KEY CLUSTERED ([Number] ASC, [AreaCode] ASC)
);

IF EXISTS (SELECT TOP 1 1 
           FROM   [dbo].[Phone])
    BEGIN
        INSERT INTO [dbo].[tmp_ms_xx_Phone] ([Number], [AreaCode], [PhoneTypeName])
        SELECT   [Number],
                 [AreaCode],
                 [PhoneTypeName]
        FROM     [dbo].[Phone]
        ORDER BY [Number] ASC, [AreaCode] ASC;
    END

DROP TABLE [dbo].[Phone];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_Phone]', N'Phone';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;


GO
PRINT N'Creating [dbo].[ClientsPhone]...';


GO
CREATE TABLE [dbo].[ClientsPhone] (
    [ClientName]    NVARCHAR (255) NOT NULL,
    [PhoneAreaCode] INT            NOT NULL,
    [PhoneNumber]   INT            NOT NULL,
    PRIMARY KEY CLUSTERED ([ClientName] ASC, [PhoneNumber] ASC, [PhoneAreaCode] ASC)
);


GO
PRINT N'Creating [dbo].[PhoneType]...';


GO
CREATE TABLE [dbo].[PhoneType] (
    [Name] NVARCHAR (255) NOT NULL,
    PRIMARY KEY CLUSTERED ([Name] ASC)
);


GO
PRINT N'Creating FK_Phone_ToPhoneType...';


GO
ALTER TABLE [dbo].[Phone] WITH NOCHECK
    ADD CONSTRAINT [FK_Phone_ToPhoneType] FOREIGN KEY ([PhoneTypeName]) REFERENCES [dbo].[PhoneType] ([Name]);


GO
PRINT N'Creating FK_ClientsPhone_ToClient...';


GO
ALTER TABLE [dbo].[ClientsPhone] WITH NOCHECK
    ADD CONSTRAINT [FK_ClientsPhone_ToClient] FOREIGN KEY ([ClientName]) REFERENCES [dbo].[Client] ([Name]);


GO
PRINT N'Creating FK_ClientsPhone_ToPhoneNumber...';


GO
ALTER TABLE [dbo].[ClientsPhone] WITH NOCHECK
    ADD CONSTRAINT [FK_ClientsPhone_ToPhoneNumber] FOREIGN KEY ([PhoneNumber], [PhoneAreaCode]) REFERENCES [dbo].[Phone] ([Number], [AreaCode]);


GO
-- Refactoring step to update target server with deployed transaction logs

IF OBJECT_ID(N'dbo.__RefactorLog') IS NULL
BEGIN
    CREATE TABLE [dbo].[__RefactorLog] (OperationKey UNIQUEIDENTIFIER NOT NULL PRIMARY KEY)
    EXEC sp_addextendedproperty N'microsoft_database_tools_support', N'refactoring log', N'schema', N'dbo', N'table', N'__RefactorLog'
END
GO
IF NOT EXISTS (SELECT OperationKey FROM [dbo].[__RefactorLog] WHERE OperationKey = '00698261-27d9-4932-985c-cf700b007c32')
INSERT INTO [dbo].[__RefactorLog] (OperationKey) values ('00698261-27d9-4932-985c-cf700b007c32')
IF NOT EXISTS (SELECT OperationKey FROM [dbo].[__RefactorLog] WHERE OperationKey = '98d2f3eb-86dc-4b42-a9ff-d479703de2b8')
INSERT INTO [dbo].[__RefactorLog] (OperationKey) values ('98d2f3eb-86dc-4b42-a9ff-d479703de2b8')
IF NOT EXISTS (SELECT OperationKey FROM [dbo].[__RefactorLog] WHERE OperationKey = 'fb8c4147-28f9-4a2b-9f50-26a77e80c6c0')
INSERT INTO [dbo].[__RefactorLog] (OperationKey) values ('fb8c4147-28f9-4a2b-9f50-26a77e80c6c0')
IF NOT EXISTS (SELECT OperationKey FROM [dbo].[__RefactorLog] WHERE OperationKey = 'e9e5ce87-0fe1-4bbc-a489-56085015797e')
INSERT INTO [dbo].[__RefactorLog] (OperationKey) values ('e9e5ce87-0fe1-4bbc-a489-56085015797e')

GO

GO
PRINT N'Checking existing data against newly created constraints';


GO
USE [$(DatabaseName)];


GO
ALTER TABLE [dbo].[Phone] WITH CHECK CHECK CONSTRAINT [FK_Phone_ToPhoneType];

ALTER TABLE [dbo].[ClientsPhone] WITH CHECK CHECK CONSTRAINT [FK_ClientsPhone_ToClient];

ALTER TABLE [dbo].[ClientsPhone] WITH CHECK CHECK CONSTRAINT [FK_ClientsPhone_ToPhoneNumber];


GO
PRINT N'Update complete.';


GO
