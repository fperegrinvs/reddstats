namespace ReddStats.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using bsparser;

    public class Transaction
    {
        public Transaction(
            UInt32 version,
            List<TxInput> inputs,
            List<TxOutput> outputs,
            UInt32 lockTime)
        {
            this.Version = version;
            this.Inputs = inputs;
            this.Outputs = outputs;
            this.LockTime = lockTime;

            var sizeEstimate = inputs.Aggregate(0L, (current, t) => current + (t.ScriptSignature.Length / 2));

            sizeEstimate = outputs.Aggregate(sizeEstimate, (current, t) => current + t.ScriptPublicKeyBinary.Length);
            sizeEstimate = (long)(sizeEstimate * 1.5);
            this.Size = sizeEstimate;
        }

        public int BlockId { get; set; }

        public uint InputsCount { get; set; }

        public uint OutputsCount { get; set; }

        public decimal TotalIn { get; set; }

        public decimal TotalOut { get; set; }

        public decimal Fee { get; set; }

        public DateTime Date { get; set; }

        public uint Version { get; set; }

        public List<TxInput> Inputs { get; set; }

        public List<TxOutput> Outputs { get; set; }

        public UInt32 LockTime { get; set; }

        private string transactionId;

        public string TransactionId
        {
            get
            {
                if (this.transactionId == null)
                {
                   this.transactionId =  DataCalculator.CalculateTransactionHash(Version, Inputs, Outputs, LockTime).ToHexStringReverse();
                }

                return this.transactionId;
            }
            set
            {
                transactionId = value;
            }
        }

        public long Size { get; set; }

        public void CalculateMetadata()
        {
            
        }
    }
}