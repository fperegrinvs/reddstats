namespace ReddStats.Core.VO
{
    using System.Collections.Generic;

    using ProtoBuf;

    [ProtoContract]
    public class Account
    {
        public Account()
        {
            RelatedAccounts = new HashSet<string>();
            Transactions = new List<AccountTransaction>();
        }

        [ProtoMember(1)]
        public string Address { get; set; }

        [ProtoMember(2)]
        public List<AccountTransaction> Transactions { get; set; }

        [ProtoMember(3)]
        public decimal Balance { get; set; }

        [ProtoMember(4)]
        public HashSet<string> RelatedAccounts { get; set; }
    }
}
