/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
USE MapperTestDb
Go

INSERT INTO Client(Name) VALUES ('Daniel Mcliver')
GO
INSERT INTO Client(Name) VALUES ('Golly Wog')
GO
INSERT INTO Client(Name) VALUES ('Ozzy Osbourne')
GO

INSERT INTO PhoneType(Name) VALUES ('Home')
GO

INSERT INTO PhoneType(Name) VALUES ('Mobile')
GO

INSERT INTO PhoneType(Name) VALUES ('Work')
GO

INSERT INTO Phone(AreaCode,Number,PhoneTypeName)
VALUES (9,8462489,'Home')
GO

INSERT INTO Phone(AreaCode,Number,PhoneTypeName)
VALUES (21,2929529,'Mobile')
GO

INSERT INTO ClientsPhone(ClientName,PhoneNumber,PhoneAreaCode) 
VALUES ('Daniel Mcliver',2929529,21)
GO

INSERT INTO ClientsPhone(ClientName,PhoneNumber,PhoneAreaCode) 
VALUES ('Daniel Mcliver',8462489,9)
GO



