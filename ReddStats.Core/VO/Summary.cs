namespace ReddStats.Core.VO
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    public class Summary
    {
        [DataMember]
        public int StartBlock { get; set; }

        [DataMember]
        public int EndBlock { get; set; }

        [DataMember]
        public long TotalTransactions { get; set; }

        [DataMember]
        public decimal TotalMoney { get; set; }

        [DataMember]
        public ulong TotalAddresses { get; set; }

        [DataMember]
        public int TotalNonZeroAddresses { get; set; }

        [DataMember]
        public List<SimpleAccount> TopAccounts { get; set; }

        [DataMember]
        public Dictionary<string, decimal> NonZero { get; set; }

        [DataMember]
        public decimal Top10Total { get; set; }

        [DataMember]
        public decimal Top100Total { get; set; }

        [DataMember]
        public decimal Average
        {
            get
            {
                return this.TotalMoney / this.TopAccounts.Count;
            } 
        }

        [DataMember]
        public decimal GniIndex { get; set; }

        [DataMember]
        public decimal TheilIndex { get; set; }
    }
}