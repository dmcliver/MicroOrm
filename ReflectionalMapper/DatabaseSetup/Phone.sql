CREATE TABLE [dbo].[Phone]
(
	[AreaCode] INT NOT NULL , 
    [Number] INT NOT NULL, 
    [PhoneTypeName] NVARCHAR(255) NOT NULL, 
    PRIMARY KEY ([Number], [AreaCode]), 
    CONSTRAINT [FK_Phone_ToPhoneType] FOREIGN KEY (PhoneTypeName) REFERENCES PhoneType(Name)
)
