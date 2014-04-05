namespace ReddStats.Core.VO
{
    using System.Collections.Generic;

    public class BlockChain
    {
        public BlockChain()
        {
            Blocks = new List<Block>();
            Transactions = new Dictionary<string, Transaction>();
        }

        public List<Block> Blocks { get; set; }

        public Dictionary<string, Transaction> Transactions { get; set; } 
    }
}
