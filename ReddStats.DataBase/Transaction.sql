CREATE TABLE [dbo].[Transaction]
(
    [TransactionId] VARCHAR(64) NOT NULL, 
    [BlockId] INT NOT NULL, 
    [InputsCount] INT NULL, 
    [OutputsCount] INT NULL, 
    [TotalIn] MONEY NULL, 
    [TotalOut] MONEY NULL, 
    [Size] INT NULL, 
    [Fee] MONEY NULL, 
    [Date] DATETIME NULL, 
    CONSTRAINT [PK_Transaction] PRIMARY KEY ([TransactionId])
)

GO

CREATE INDEX [IX_Block] ON [dbo].[Transaction] (BlockId)
