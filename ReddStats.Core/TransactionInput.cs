namespace ReddStats.Core
{
    using ProtoBuf;

    [ProtoContract]
    public class TransactionInput
    {
        [ProtoMember(1)]
        public int BlockId { get; set; }

        [ProtoMember(2)]
        public string TransactionId { get; set; }

        [ProtoMember(3)]
        public int Index { get; set; }

        [ProtoMember(4)]
        public string PreviousOutputKey { get; set; }

        [ProtoMember(5)]
        public int PreviousOutputIndex { get; set; }

        [ProtoMember(6)]
        public string ScriptSignature { get; set; }

        [ProtoMember(7)]
        public string FromAddress { get; set; }

        [ProtoMember(8)]
        public long Sequence { get; set; }

        [ProtoMember(9)]
        public decimal Amount { get; set; }

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