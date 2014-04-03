namespace ReddStats.Core
{
    using System;

    using bsparser;

    public class TransactionOutput
    {
        public int BlockId { get; set; }

        public string TransactionId { get; set; }

        public int Index { get; set; }

        public Decimal Amount { get; set; }

        public byte[] ScriptPublicKeyBinary { get; set; }

        public string ScriptPublicKey { get; set; }

        private string toAddress;

        public string ToAddress
        {
            get
            {
                if (toAddress == null)
                {
                    if (Amount > 0)
                    {
                        toAddress = DataCalculator.GetToAddress(this.ScriptPublicKeyBinary);
                    }
                    else
                    {
                        toAddress = "Unknown";
                    }
                }

                return toAddress;
            }
        }
    }
}