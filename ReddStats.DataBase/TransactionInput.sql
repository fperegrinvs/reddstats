CREATE TABLE [dbo].[TransactionInput]
(
	[TransactionId] VARCHAR(64) NOT NULL , 
    [Order] INT NOT NULL, 
    [PreviousOutput] VARCHAR(64) NOT NULL, 
    [Ammount] MONEY NULL, 
    [Date] DATETIME NULL, 
    [BlockId] INT NOT NULL, 
    [FromAddress] VARCHAR(34) NULL, 
    [ScriptSig] VARCHAR(MAX) NULL, 
    PRIMARY KEY ([Order], [TransactionId])
)

GO

CREATE INDEX [IX_Key] ON [dbo].[TransactionInput] (FromAddress)

GO

CREATE INDEX [IX_Block] ON [dbo].[TransactionInput] (BlockId)

GO
