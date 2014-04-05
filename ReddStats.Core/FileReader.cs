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

    public class FileReader
    {
        public static List<Block> ParseChain()
        {
            var basePath = ConfigurationManager.AppSettings["wallet.datapath"];

            if (!Directory.Exists(basePath))
            {
                throw new Exception("Invalid wallet data path");
            }

            var files = Directory.GetFiles(basePath + "\\blocks", "blk*.dat").OrderBy(x => x);

            int i = 0;

            var blocks = new List<Block>();

            var memory = new MemoryProvider();

            foreach (var file in files)
            {
                var fileBlocks = ParseFile(file, i, memory, memory);
                if (fileBlocks == null)
                {
                    break;
                }

                blocks.AddRange(fileBlocks);
            }


            return blocks;
        }

        public static IEnumerable<Block> ParseFile(string filename, int startBlock, ITransactionProvider transactionProvider, IBlockProvider blockProvider)
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
                block = BlockParser.ReadBlock(stream, startBlock++, transactionProvider);

                if (block != null)
                {
                    var blockCount = blockProvider.GetBlockCount();
                    if (blockCount > 0)
                    {
                        var lastBlock = blockProvider.GetBlock(blockCount - 1);
                        lastBlock.Hash = block.PreviousBlockHash;

                        if (blockCount > 1)
                        {
                            var otherBlock = blockProvider.GetBlock(blockCount - 2);
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
