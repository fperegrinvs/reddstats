using System;

namespace ReddStats.Core.Providers
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading;

    using ReddStats.Core.Interface;
    using ReddStats.Core.VO;

    public class CassandraProvider : IBlockChainDataProvider
    {
        public static event BlockChangedHandler BlockChanged;

        public static event AccountChangedHandler AccountChanged;

        public static event ConsolidateChangedHandler ConsolidatedChanged;

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

        public Account GetAccount(string address)
        {
            return db.Get<Account>(address, "account");
        }

        public RelatedAccounts GetRealtedAccounts()
        {
            return db.Get<RelatedAccounts>("related", "account");
        }

        public void SaveRelatedAccounts(RelatedAccounts related)
        {
            db.Insert("related", "account", related);
        }

        public ConsolidatedData GetConsolidatedData(int endBlock)
        {
            return db.Get<ConsolidatedData>(endBlock.ToString(CultureInfo.InvariantCulture), "consolidated");
        }

        public void SaveBlock(Block block)
        {
            db.Insert(block.Id.ToString(CultureInfo.InvariantCulture), "block", block);

            if (BlockChanged != null)
            {
                BlockChanged(this, block);
            }
        }

        public void SaveBlockCount(int count)
        {
            this.db.InsertValueNameValue("info", "block", "blockcount", count.ToString(CultureInfo.InvariantCulture));
        }

        public void SaveAccount(Account account)
        {
            db.Insert(account.Address, "account", account);

            if (AccountChanged != null)
            {
                AccountChanged(this, account);
            }
        }

        public void SaveConsolidatedData(ConsolidatedData data)
        {
            db.Insert(data.EndBlock.ToString(CultureInfo.InvariantCulture), "consolidated", data);

            if (ConsolidatedChanged != null)
            {
                ConsolidatedChanged(this, data);
            }
        }

        public void SaveAllAccounts(Dictionary<string, Account> accounts)
        {
            var data = accounts.Values.ToList();
            
            // clean transactions to save space
            foreach (var account in data)
            {
                account.Transactions = null;
            }

            db.Insert("all", "account", data);
        }

        public Dictionary<string, Account> GetAllAccounts()
        {
            var data = db.Get<List<Account>>("all", "account");

            return data.ToDictionary(a => a.Address);
        }

        public BlockChain ImportFromFiles()
        {
            var chain = FileReader.ParseChain();
            foreach (var block in chain.Blocks)
            {
                this.SaveBlock(block);
            }

            foreach (var transaction in chain.Transactions)
            {
                this.SaveTransaction(transaction.Value);
            }

            this.SaveBlockCount(chain.Blocks.Count);

            return chain;
        }

        public void ConsolidateAllData(BlockChain chain)
        {
            var gap = 60 * 24;
            ConsolidatedData consolidated = null;

            for (var start = 0; start < chain.Blocks.Count; start += gap)
            {
                consolidated = DataProcessor.ConsolidateData(chain, consolidated, start, start - 1 +  gap);
                this.SaveConsolidatedData(consolidated);
            }

            if (consolidated != null && (consolidated.EndBlock + 1) % gap != 0)
            {
                consolidated = DataProcessor.ConsolidateData(chain, consolidated, consolidated.EndBlock + 1);
            }

            if (consolidated != null)
            {
                foreach (var account in consolidated.Accounts)
                {
                    this.SaveAccount(account.Value);
                }

                this.SaveRelatedAccounts(consolidated.RelatedAccounts);
                this.SaveAllAccounts(consolidated.Accounts);
            }
        }

        public static void KeepUpdated()
        {
            var cassandra = new CassandraProvider();
            var updateThread = new Thread(cassandra.KeepUpdatedThread);
            updateThread.Start();
        }

        private void KeepUpdatedThread()
        {
            var lastblock = this.GetBlockCount() - 1;
            var rpc = new RpcProvider();

            var lastIndex = ((lastblock / 1440) * 1440) - 1; 

           ConsolidatedData consolidated = this.GetConsolidatedData(lastIndex);
            var related = this.GetRealtedAccounts();
            var accounts = this.GetAllAccounts();
            consolidated.Accounts = accounts;
            consolidated.RelatedAccounts = related;

            while (true)
            {
                var rpcCount = rpc.GetBlockCount();

                if (rpcCount > lastblock + 1)
                {
                    for (var blockToGet = lastIndex; blockToGet < rpcCount; blockToGet++)
                    {
                        
                        var block = rpc.GetBlock(blockToGet);
                        this.SaveBlock(block);
                        foreach (var transaction in block.Transactions)
                        {
                            this.SaveTransaction(transaction);
                        }

                        var chain = new BlockChain
                                     {
                                         Blocks = new List<Block> { block },
                                         Transactions = block.Transactions.ToDictionary(t => t.TransactionId)
                                     };

                        consolidated = DataProcessor.ConsolidateData(chain, consolidated, blockToGet, blockToGet, this);

                        if ((blockToGet + 1) % 1440 == 0)
                        {
                            this.SaveConsolidatedData(consolidated);
                        }
                    }

                    foreach (var account in consolidated.Accounts.Values.Where(a => a.Changed))
                    {
                        account.Changed = false;
                        this.SaveAccount(account);
                    }

                    this.SaveAllAccounts(consolidated.Accounts);
                    this.SaveBlockCount(rpcCount);

                    lastIndex = rpcCount - 1;
                }

                Thread.Sleep(10000);
            }
        }
    }

    public delegate void BlockChangedHandler(object sender, Block block);

    public delegate void AccountChangedHandler(object sender, Account account);

    public delegate void ConsolidateChangedHandler(object sender, ConsolidatedData consolidated);
}
