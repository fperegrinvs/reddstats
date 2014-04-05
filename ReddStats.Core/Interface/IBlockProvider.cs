namespace ReddStats.Core.Interface
{
    public interface IBlockChainDataProvider
    {
        int GetBlockCount();

        Block GetBlock(int id);

        void SaveBlock(Block block);

        TransactionOutput GetTransactionOutput(string transactionId, int order);

        TransactionInput GetTransactionInput(string transactionId, int order);

        Transaction GetTransaction(string transactionId);

        void SaveTransaction(Transaction transaction);
    }
}
