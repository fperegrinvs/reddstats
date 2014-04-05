using System;

namespace ReddStats.Core.Providers
{
    using System.IO;

    using ReddStats.Core.Interface;
    using ReddStats.Core.Parser;
    using ReddStats.Core.RPC;
    using ReddStats.Core.VO;

    public class RpcProvider : IBlockChainDataProvider
    {
        public TransactionOutput GetTransactionOutput(string transactionId, int order)
        {
            throw new NotImplementedException();
        }

        public TransactionInput GetTransactionInput(string transactionId, int order)
        {
            throw new NotImplementedException();
        }

        public Transaction GetTransaction(string transactionId)
        {
            throw new NotImplementedException();
        }

        public void SaveTransaction(Transaction transaction)
        {
            throw new NotImplementedException();
        }

        public int GetBlockCount()
        {
            return RpcClient.DoRequest<int>("getblockcount");
        }

        public Block GetBlock(int index)
        {
            var hash = RpcClient.DoRequest<string>("getblockhash", new Object[] { index });
            var bits = RpcClient.DoRequest<string>("getblock", new object[] { hash, false }).FromHexString();
            using (var ms = new MemoryStream(bits))
            {
                var block = BlockParser.ReadBlock(ms, index, new CassandraProvider());
                return block;
            }
        }

        public void SaveBlock(Block block)
        {
            throw new NotImplementedException();
        }
    }
}
