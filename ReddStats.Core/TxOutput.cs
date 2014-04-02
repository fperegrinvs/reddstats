namespace ReddStats.Core
{
    using System;

    public class TxOutput
    {
        public int BlockId { get; set; }

        public string TransactionId { get; set; }

        public int Index { get; set; }

        public Decimal Value { get; set; }

        public byte[] ScriptPublicKeyBinary { get; set; }

        public string ScriptPublicKey { get; set; }
    }
}