namespace ReddStats.Core.VO
{
    using System.Collections.Generic;

    using ProtoBuf;

    [ProtoContract]
    public class ConsolidatedData
    {
        [ProtoMember(1)]
        public int EndBlock { get; set; }

        [ProtoMember(2)]
        public Dictionary<string, decimal> AccountBalances { get; set; }

        [ProtoMember(3)]
        public decimal TotalMoney { get; set; }

        [ProtoMember(4)]
        public long TotalTransactions { get; set; }

        [ProtoMember(5)]
        public decimal TotalTransactionsValue { get; set; }

        [ProtoIgnore]
        public Dictionary<string, Account> Accounts { get; set; }

        [ProtoIgnore]
        public RelatedAccounts RelatedAccounts { get; set; }
    }
}
