using System.Collections.Generic;

namespace ReddStats.Core.VO
{
    using System.Runtime.Serialization;

    using ProtoBuf;

    [DataContract]
    [ProtoContract]
    public class RelatedAccounts
    {
        public RelatedAccounts()
        {
            LinkedAccountList = new Dictionary<string, HashSet<string>>();
            LinkedAccounts = new Dictionary<string, string>();
        }

        [DataMember]
        [ProtoMember(1)]
        public Dictionary<string, string> LinkedAccounts { get; set; }

        [DataMember]
        [ProtoMember(2)]
        public Dictionary<string, HashSet<string>> LinkedAccountList { get; set; }

        public string GetMainAccount(string address)
        {
            if (LinkedAccountList.ContainsKey(address))
            {
                return address;
            }

            if (LinkedAccounts.ContainsKey(address))
            {
                return LinkedAccounts[address];
            }

            return address;
        }

        public void AddRelatedAccount(string main, string related)
        {
            LinkedAccounts[related] = main;

            if (!LinkedAccountList.ContainsKey(main))
            {
                LinkedAccountList[main] = new HashSet<string>();
            }

            LinkedAccountList[main].Add(related);
        }
    }
}
