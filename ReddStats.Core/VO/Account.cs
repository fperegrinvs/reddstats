namespace ReddStats.Core.VO
{
    using System.Collections.Generic;

    using ProtoBuf;

    [ProtoContract]
    public class Account
    {
        [ProtoMember(1)]
        public string Address { get; set; }

        [ProtoMember(2)]
        public List<AccountTransaction> Transactions { get; set; }

        [ProtoMember(3)]
        public decimal Balance { get; set; }

        [ProtoMember(4)]
        public List<string> RelatedAccounts { get; set; }
    }
}
