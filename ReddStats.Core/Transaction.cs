namespace ReddStats.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using bsparser;

    public class Transaction
    {
        public Transaction(
            long version,
            List<TransactionInput> inputs,
            List<TransactionOutput> outputs,
            long lockTime,
            int blockId)
        {
            this.Version = version;
            this.Inputs = inputs;
            this.Outputs = outputs;
            this.LockTime = lockTime;
            this.BlockId = blockId;
            var sizeEstimate = inputs.Aggregate(0L, (current, t) => current + (t.ScriptSignature.Length / 2));

            sizeEstimate = outputs.Aggregate(sizeEstimate, (current, t) => current + t.ScriptPublicKeyBinary.Length);
            sizeEstimate = (long)(sizeEstimate * 1.5);
            this.Size = sizeEstimate;

            this.TotalOut = 0M;
            this.TotalIn = 0M;

            for (var k = 0; k < inputs.Count; k++)
            {
                inputs[k].Index = k;
                inputs[k].TransactionId = TransactionId;
                inputs[k].BlockId = BlockId;
                TotalIn += inputs[k].Amount;
            }

            for (var k = 0; k < outputs.Count; k++)
            {
                outputs[k].Index = k;
                outputs[k].TransactionId = TransactionId;
                outputs[k].BlockId = BlockId;
                TotalOut += outputs[k].Amount;
            }
        }

        public int BlockId { get; set; }

        public int InputsCount
        {
            get
            {
                return Inputs.Count;
            }
        }

        public int OutputsCount
        {
            get
            {
                return Outputs.Count;
            }
        }

        public decimal TotalIn { get; set; }

        public decimal TotalOut { get; private set; }

        public decimal Fee
        {
            get
            {
                return TotalIn == 0 ? 0M : TotalIn - TotalOut;
            }
        }

        public DateTime? Date { get; set; }

        public long Version { get; set; }

        public List<TransactionInput> Inputs { get; set; }

        public List<TransactionOutput> Outputs { get; set; }

        public long LockTime { get; set; }

        private string transactionId;

        public string TransactionId
        {
            get
            {
                if (this.transactionId == null)
                {
                   this.transactionId =  DataCalculator.CalculateTransactionHash((uint)Version, Inputs, Outputs, (uint)LockTime).ToHexStringReverse();
                }

                return this.transactionId;
            }
            set
            {
                transactionId = value;
            }
        }

        public long Size { get; private set; }
    }
}