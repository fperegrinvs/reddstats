namespace ReddStats.Core
{
    using System.Runtime.Serialization;

    using ProtoBuf;

    [DataContract]
    [ProtoContract]
    public class TransactionInput
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
        public string PreviousOutputKey { get; set; }

        [DataMember]
        [ProtoMember(5)]
        public int PreviousOutputIndex { get; set; }

        [DataMember]
        [ProtoMember(6)]
        public string ScriptSignature { get; set; }

        [DataMember]
        [ProtoMember(7)]
        public string FromAddress { get; set; }

        [DataMember]
        [ProtoMember(8)]
        public long Sequence { get; set; }

        [DataMember]
        [ProtoMember(9)]
        public decimal Amount { get; set; }

        [DataMember]
        [ProtoMember(10)]
        public byte[] PreviousTxOutputKeyBinary { get; set; }


        private string previouTxReference;

        [ProtoMember(11)]
        public string PreviousTxReference
        {
            get
            {
                if (previouTxReference == null)
                {
                    previouTxReference = this.PreviousOutputKey + "#" + PreviousOutputIndex;
                }

                return previouTxReference;
            }
            set
            {
                previouTxReference = value;
            }
        }
    }
}