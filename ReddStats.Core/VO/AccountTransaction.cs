using System;

namespace ReddStats.Core.VO
{
    using ProtoBuf;

    [ProtoContract]
    public class AccountTransaction
    {
        [ProtoMember(1)]
        public string TransactionId { get; set; }

        [ProtoMember(2)]
        public int TransactionIndex { get; set; }

        [ProtoMember(3)]
        public TransactionTypeEnum TransactionType { get; set; }

        [ProtoMember(4)]
        public int BlockId { get; set; }

        [ProtoMember(5)]
        public DateTime Date { get; set; }

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
