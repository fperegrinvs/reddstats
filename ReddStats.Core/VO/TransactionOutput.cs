namespace ReddStats.Core.VO
{
    using System;

    using ProtoBuf;

    using ReddStats.Core.Parser;

    [ProtoContract]
    public class TransactionOutput
    {
        [ProtoMember(1)]
        public int BlockId { get; set; }

        [ProtoMember(2)]
        public string TransactionId { get; set; }

        [ProtoMember(3)]
        public int Index { get; set; }

        [ProtoMember(4)]
        public Decimal Amount { get; set; }

        [ProtoMember(5)]
        public byte[] ScriptPublicKeyBinary { get; set; }

        [ProtoMember(6)]
        public string ScriptPublicKey { get; set; }

        private string toAddress;

        [ProtoMember(7)]
        public string ToAddress
        {
            get
            {
                if (this.toAddress == null)
                {
                    if (this.Amount > 0)
                    {
                        this.toAddress = DataCalculator.GetToAddress(this.ScriptPublicKeyBinary);
                    }
                    else
                    {
                        this.toAddress = "Unknown";
                    }
                }

                return this.toAddress;
            }
            set
            {
                this.toAddress = value;
            }
        }
    }
}