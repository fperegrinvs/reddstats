using System;

namespace ReddStats.Core.Providers
{
    using System.Globalization;

    using ReddStats.Core.Interface;

    public class CassandraProvider : ITransactionProvider, IBlockProvider
    {
        private readonly BlockChainDb db = new BlockChainDb();

        public TransactionOutput GetTransactionOutput(string transactionId, int order)
        {
            if (transactionId == "0000000000000000000000000000000000000000000000000000000000000000")
            {
                return null;
            }

            return db.Get<Transaction>(transactionId, "transaction").Outputs[order];
        }

        public TransactionInput GetTransactionInput(string transactionId, int order)
        {
            return db.Get<Transaction>(transactionId, "transaction").Inputs[order];
        }

        public Transaction GetTransaction(string transactionId)
        {
            return db.Get<Transaction>(transactionId, "transaction");
        }

        public void SaveTransaction(Transaction transaction)
        {
            db.Insert(transaction.TransactionId, "transaction", transaction);
        }

        public int GetBlockCount()
        {
            return Convert.ToInt32(db.GetValue("info", "blockcount", "block"));
        }

        public Block GetBlock(int id)
        {
            return db.Get<Block>(id.ToString(CultureInfo.InvariantCulture), "block");
        }

        public void SaveBlock(Block block)
        {
            db.Insert(block.Id.ToString(CultureInfo.InvariantCulture), "block", block);
        }
    }
}
