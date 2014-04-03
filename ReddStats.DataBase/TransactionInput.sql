CREATE TABLE [dbo].[TransactionInput] (
    [TransactionId]       VARCHAR (64)  NOT NULL,
    [Index]               INT           NOT NULL,
    [PreviousOutputKey]   VARCHAR (64)  NULL,
    [PreviousOutputIndex] BIGINT        NULL,
    [Amount]              MONEY         NULL,
    [Date]                DATETIME      NULL,
    [BlockId]             INT           NOT NULL,
    [FromAddress]         VARCHAR (34)  NULL,
    [Sequence]            BIGINT        NOT NULL,
    [ScriptSignature]     VARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK__Transact__C2F7EBCB94142CC2] PRIMARY KEY CLUSTERED ([Index] ASC, [TransactionId] ASC)
);



GO

CREATE INDEX [IX_Key] ON [dbo].[TransactionInput] (FromAddress)

GO

CREATE INDEX [IX_Block] ON [dbo].[TransactionInput] (BlockId)

GO
