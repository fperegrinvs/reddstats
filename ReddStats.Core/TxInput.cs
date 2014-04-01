namespace ReddStats.Core
{
    using System.Collections.Generic;

    public class TxInput
    {
        public string  PreviousTxOutputKey { get; set; }

        public uint PreviousOutputIndex { get; set; }

        public string ScriptSignature { get; set; }

        public uint Sequence { get; set; }

        public byte[] PreviousTxOutputKeyBinary { get; set; }

        private string previouTxReference;

        public string PreviousTxReference
        {
            get
            {
                if (previouTxReference == null)
                {
                    previouTxReference = PreviousTxOutputKey + "#" + PreviousOutputIndex;
                }

                return previouTxReference;
            }
        }
    }
}