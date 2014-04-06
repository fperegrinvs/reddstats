namespace ReddStats.Core.VO
{
    using System.Collections.Generic;

    using ProtoBuf;

    [ProtoContract]
    public class Account
    {
        private decimal balance;

        public bool Changed { get; set; }

        public Account()
        {
            RelatedAccounts = new HashSet<string>();
            Transactions = new HashSet<AccountTransaction>();
        }

        [ProtoMember(1)]
        public string Address { get; set; }

        [ProtoMember(2)]
        public HashSet<AccountTransaction> Transactions { get; set; }

        [ProtoMember(3)]
        public HashSet<string> RelatedAccounts { get; set; }
    }
}
