CREATE TABLE [dbo].[ClientsPhone]
(
	[ClientName] NVARCHAR(255) NOT NULL , 
    [PhoneAreaCode] INT NOT NULL, 
    [PhoneNumber] INT NOT NULL, 
    PRIMARY KEY ([ClientName], [PhoneNumber], [PhoneAreaCode]), 
    CONSTRAINT [FK_ClientsPhone_ToClient] FOREIGN KEY (ClientName) REFERENCES Client(Name), 
    CONSTRAINT [FK_ClientsPhone_ToPhoneNumber] FOREIGN KEY (PhoneNumber,PhoneAreaCode) REFERENCES Phone(Number,AreaCode), 
)
