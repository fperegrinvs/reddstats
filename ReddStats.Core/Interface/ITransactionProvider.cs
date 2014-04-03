namespace ReddStats.Core.Interface
{
    interface ITransactionProvider
    {
        TransactionOutput GetTransactionOutput(string transactionId, int order);

        TransactionInput GetTransactionInput(string transactionId, int order);

        Transaction GetTransaction(string transactionId);

        void SaveTransaction(Transaction transaction);
    }
}
