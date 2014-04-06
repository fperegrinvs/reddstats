namespace ReddStats.Core.VO
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    using ProtoBuf;

    [DataContract]
    [ProtoContract]
    public class Account
    {
        public bool Changed { get; set; }

        public Account()
        {
            RelatedAccounts = new HashSet<string>();
            Transactions = new HashSet<AccountTransaction>();
        }

        [DataMember]
        [ProtoMember(1)]
        public string Address { get; set; }

        [DataMember]
        [ProtoMember(2)]
        public HashSet<AccountTransaction> Transactions { get; set; }

        [ProtoMember(3)]
        [DataMember]
        public HashSet<string> RelatedAccounts { get; set; }
    }
}
