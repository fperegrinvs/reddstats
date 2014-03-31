namespace bsparser
{
    using System;
    using System.Collections.Generic;

    public class TxOutput
    {
        public TxOutput(UInt64 value, List<byte> scriptPublicKey)
        {
            this.Value = value;
            this.ScriptPublicKey = scriptPublicKey;
        }

        public UInt64 Value { get; private set; }

        public List<byte> ScriptPublicKey { get; private set; }
    }
}