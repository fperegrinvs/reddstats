namespace bsparser
{
    using System;
    using System.Collections.Generic;

    public class TxInput
    {
        public TxInput(TxOutputKey previousTxOutputKey, List<byte> scriptSignature, UInt32 sequence)
        {
            this.PreviousTxOutputKey = previousTxOutputKey;
            this.ScriptSignature = scriptSignature;
            this.Sequence = sequence;
        }

        public TxOutputKey PreviousTxOutputKey { get; private set; }

        public List<byte> ScriptSignature { get; private set; }

        public UInt32 Sequence { get; private set; }
    }
}