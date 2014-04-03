CREATE TABLE [dbo].[TransactionOutput] (
    [TransactionId]   VARCHAR (64)  NOT NULL,
    [Index]           INT           NOT NULL,
    [RedeemedAt]      VARCHAR (64)  NULL,
    [BlockId]         INT           NOT NULL,
    [Date]            DATETIME      NULL,
    [ToAddress]       VARCHAR (34)  NOT NULL,
    [Amount]          MONEY         NOT NULL,
    [ScriptPublicKey] VARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK__Transact__23F69A993AD73CF9] PRIMARY KEY CLUSTERED ([Index] ASC, [TransactionId] ASC)
);



GO

CREATE INDEX [IX_ToAddress] ON [dbo].[TransactionOutput] (ToAddress)

GO

CREATE INDEX [IX_Block] ON [dbo].[TransactionOutput] (BlockId)
