CREATE TABLE [dbo].[Block] (
    [Id]                    INT          NOT NULL,
    [Date]                  DATETIME     NOT NULL,
    [Version]               BIGINT       NOT NULL,
    [PreviousBlockHash]     VARCHAR (64) NULL,
    [MerkleRoot]            VARCHAR (64) NOT NULL,
    [NextBlockHash]         VARCHAR (64) NULL,
    [Difficulty]            FLOAT (53)   NOT NULL,
    [Nonce]                 BIGINT       NOT NULL,
    [Hash]                  VARCHAR (64) NOT NULL,
    [HashRate]              MONEY        NULL,
    [Size]                  BIGINT       NULL,
    [TransactionsCount]     INT          NOT NULL,
    [TotalTransactionValue] MONEY        NOT NULL,
    [TotalFees]             MONEY        NULL,
    [CoinsCreated]          MONEY        NOT NULL,
    CONSTRAINT [PK__tmp_ms_x__3214EC072514762F] PRIMARY KEY CLUSTERED ([Id] ASC)
);


