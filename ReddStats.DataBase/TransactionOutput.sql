CREATE TABLE [dbo].[TransactionOutput]
(
	[IdTransaction] VARCHAR(64) NOT NULL , 
    [Order] INT NOT NULL, 
    [RedeemedAt] VARCHAR(64) NULL, 
    [BlockId] INT NOT NULL, 
    [Date] DATETIME NOT NULL, 
    [ToAddress] VARCHAR(34) NOT NULL, 
    [Ammount] MONEY NOT NULL, 
    [ScriptPubKey] VARCHAR(MAX) NOT NULL, 
    PRIMARY KEY ([Order], [IdTransaction])
)

GO

CREATE INDEX [IX_ToAddress] ON [dbo].[TransactionOutput] (ToAddress)

GO

CREATE INDEX [IX_Block] ON [dbo].[TransactionOutput] (BlockId)
