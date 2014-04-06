using System;

namespace ReddStats.Core.VO
{
    using System.Runtime.Serialization;

    using ProtoBuf;

    [DataContract]
    [ProtoContract]
    public class AccountTransaction
    {
        [DataMember]
        [ProtoMember(1)]
        public string TransactionId { get; set; }

        [DataMember]
        [ProtoMember(2)]
        public int TransactionIndex { get; set; }

        [DataMember]
        [ProtoMember(3)]
        public TransactionTypeEnum TransactionType { get; set; }

        [DataMember]
        [ProtoMember(4)]
        public int BlockId { get; set; }

        [DataMember]
        [ProtoMember(5)]
        public DateTime Date { get; set; }

        [DataMember]
        [ProtoMember(6)]
        public Decimal Value { get; set; }

        public override int GetHashCode()
        {
            return (TransactionIndex + TransactionType + TransactionId).GetHashCode();
        }
    }

    [ProtoContract]
    public enum TransactionTypeEnum
    {
        Input = 0,
        Output = 1,
    }
}
