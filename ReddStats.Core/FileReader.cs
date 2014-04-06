using System.Collections.Generic;

namespace ReddStats.Core
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Linq;

    using ReddStats.Core.Interface;
    using ReddStats.Core.Parser;
    using ReddStats.Core.Providers;
    using ReddStats.Core.VO;

    public class FileReader
    {
        public static BlockChain ParseChain()
        {
            var basePath = ConfigurationManager.AppSettings["wallet.datapath"];

            if (!Directory.Exists(basePath))
            {
                throw new Exception("Invalid wallet data path");
            }

            var files = Directory.GetFiles(basePath + "\\blocks", "blk*.dat").OrderBy(x => x);

            int startBlock = 0;

            var chain = new BlockChain();

            var memory = new MemoryProvider();

            foreach (var file in files)
            {
                var fileBlocks = ParseFile(file, startBlock, memory);

                if (fileBlocks == null)
                {
                    break;
                }

                startBlock += fileBlocks.Count;

                chain.Blocks.AddRange(fileBlocks);

                foreach (var block in fileBlocks)
                {
                    foreach (var transaction in block.Transactions)
                    {
                        chain.Transactions[transaction.TransactionId] = transaction;
                    }
                }
            }

            return chain;
        }

        public static List<Block> ParseFile(string filename, int startBlock, IBlockChainDataProvider dataProvider)
        {
            if (!File.Exists(filename))
            {
                return null;
            }

            var stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            var result = new List<Block>();

            Block block;
            do
            {
                block = BlockParser.ReadBlock(stream, startBlock++, dataProvider);

                if (block != null)
                {
                    var blockCount = dataProvider.GetBlockCount();
                    if (blockCount > 0)
                    {
                        var lastBlock = dataProvider.GetBlock(blockCount - 1);
                        lastBlock.Hash = block.PreviousBlockHash;

                        if (blockCount > 1)
                        {
                            var otherBlock = dataProvider.GetBlock(blockCount - 2);
                            otherBlock.NextBlockHash = lastBlock.Hash;
                        }
                    }

                    result.Add(block);
                }
            }
            while (block != null);

            return result;
        }
    }
}
