namespace ReddStats.Core.VO
{
    using System;
    using System.Runtime.Serialization;

    using ProtoBuf;

    using ReddStats.Core.Parser;

    [DataContract]
    [ProtoContract]
    public class TransactionOutput
    {
        [DataMember]
        [ProtoMember(1)]
        public int BlockId { get; set; }

        [DataMember]
        [ProtoMember(2)]
        public string TransactionId { get; set; }

        [DataMember]
        [ProtoMember(3)]
        public int Index { get; set; }

        [DataMember]
        [ProtoMember(4)]
        public Decimal Amount { get; set; }

        [DataMember]
        [ProtoMember(5)]
        public byte[] ScriptPublicKeyBinary { get; set; }

        [DataMember]
        [ProtoMember(6)]
        public string ScriptPublicKey { get; set; }

        private string toAddress;

        [DataMember]
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