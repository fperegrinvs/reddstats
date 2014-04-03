namespace ReddStats.Core
{
    public class TransactionInput
    {
        public int BlockId { get; set; }

        public string TransactionId { get; set; }

        public int Index { get; set; }

        public string  PreviousOutputKey { get; set; }

        public int PreviousOutputIndex { get; set; }

        public string ScriptSignature { get; set; }

        public string FromAddress { get; set; }

        public long Sequence { get; set; }

        public decimal Amount { get; set; }

        public byte[] PreviousTxOutputKeyBinary { get; set; }


        private string previouTxReference;

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
        }
    }
}