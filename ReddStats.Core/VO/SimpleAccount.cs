namespace ReddStats.Core.VO
{
    using System.Runtime.Serialization;

    [DataContract]
    public class SimpleAccount
    {
        [DataMember]
        public string Address { get; set; }

        [DataMember]
        public decimal Balance { get; set; }
    }
}