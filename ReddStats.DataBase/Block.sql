CREATE TABLE [dbo].[Block]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Date] DATETIME NOT NULL, 
    [Version] INT NOT NULL, 
    [PreviousBlockHash] VARCHAR(64) NULL,
	[MerkleRoot] VARCHAR (64) NOT NULL,
    [NextBlockHash] VARCHAR(64) NULL, 
    [Difficulty] FLOAT NOT NULL, 
    [Nonce] INT NOT NULL, 
    [Hash] VARCHAR(64) NOT NULL, 
    [HashRate] MONEY NULL, 
    [Size] INT NULL, 
    [TransactionsCount] INT NOT NULL, 
    [TotalTransactionValue] MONEY NOT NULL, 
    [TotalFees] MONEY NOT NULL, 
    [CoinsCreated] MONEY NOT NULL
)
