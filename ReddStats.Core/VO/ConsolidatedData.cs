namespace ReddStats.Core.VO
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    using ProtoBuf;

    [DataContract]
    [ProtoContract]
    public class ConsolidatedData
    {
        [DataMember]
        [ProtoMember(1)]
        public int EndBlock { get; set; }

        [DataMember]
        [ProtoMember(2)]
        public Dictionary<string, decimal> AccountBalances { get; set; }

        [DataMember]
        [ProtoMember(3)]
        public decimal TotalMoney { get; set; }

        [DataMember]
        [ProtoMember(4)]
        public long TotalTransactions { get; set; }

        [DataMember]
        [ProtoMember(5)]
        public decimal TotalTransactionsValue { get; set; }

        [DataMember]
        [ProtoIgnore]
        public Dictionary<string, Account> Accounts { get; set; }

        [DataMember]
        [ProtoIgnore]
        public RelatedAccounts RelatedAccounts { get; set; }

        public void RemoveZeroBalances()
        {
            AccountBalances = AccountBalances.Where(a => a.Value > 0M).ToDictionary(a => a.Key, b => b.Value);
        }
    }
}
