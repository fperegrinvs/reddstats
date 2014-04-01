CREATE TABLE [dbo].[Address]
(
	[Address] VARCHAR(34) NOT NULL PRIMARY KEY, 
    [IsPrimary] BIT NOT NULL, 
    [PrimaryAddress] VARCHAR(34) NULL, 
    [Meta] VARCHAR(255) NULL
)

GO

CREATE INDEX [IX_Primary] ON [dbo].[Address] (PrimaryAddress
)

GO

CREATE INDEX [IsPrimary] ON [dbo].[Address] (IsPrimary)
