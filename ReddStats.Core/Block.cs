namespace bsparser
{
    using System.Collections.Generic;

    public class Block
    {
        public BlockHeader Header { get; set; }
        public List<Transaction> Transactions { get; set; }
    }
}