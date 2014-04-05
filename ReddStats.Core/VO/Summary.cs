namespace ReddStats.Core.VO
{
    using System.Collections.Generic;

    public class Summary
    {
        public int StartBlock { get; set; }

        public int EndBlock { get; set; }

        public long TotalTransactions { get; set; }

        public decimal TotalMoney { get; set; }

        public ulong TotalAddresses { get; set; }

        public int TotalNonZeroAddresses { get; set; }

        public List<SimpleAccount> TopAccounts { get; set; }

        public Dictionary<string, decimal> NonZero { get; set; } 

        public decimal Top10Total { get; set; }

        public decimal Top100Total { get; set; }

        public decimal Average
        {
            get
            {
                return this.TotalMoney / this.TopAccounts.Count;
            } 
        }

        public decimal GniIndex { get; set; }

        public decimal TheilIndex { get; set; }
    }
}