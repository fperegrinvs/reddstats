namespace ReddStats.Core.Providers
{
    using System.Collections.Generic;

    using ReddStats.Core.Interface;

    public class MemoryProvider : ITransactionProvider, IBlockProvider
    {
        protected Dictionary<int, Block> Blocks = new Dictionary<int, Block>();

        protected Dictionary<string, Transaction> Transactions = new Dictionary<string, Transaction>();

        public TransactionOutput GetTransactionOutput(string transactionId, int order)
        {
            if (Transactions.ContainsKey(transactionId))
            {
                return Transactions[transactionId].Outputs[order];
            }

            return null;
        }

        public TransactionInput GetTransactionInput(string transactionId, int order)
        {
            if (Transactions.ContainsKey(transactionId))
            {
                return Transactions[transactionId].Inputs[order];
            }

            return null;
        }

        public Transaction GetTransaction(string transactionId)
        {
            if (Transactions.ContainsKey(transactionId))
            {
                return Transactions[transactionId];
            }

            return null;
        }

        public void SaveTransaction(Transaction transaction)
        {
            Transactions[transaction.TransactionId] = transaction;
        }

        public int GetBlockCount()
        {
            return Blocks.Count;
        }

        public Block GetBlock(int id)
        {
            if (Blocks.Count > id)
            {
                return Blocks[id];
            }

            return null;
        }

        public void SaveBlock(Block block)
        {
            Blocks[block.Id] = block;
        }
    }
}
