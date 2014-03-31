namespace bsparser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Transaction
    {
        public Transaction(
            UInt32 version,
            List<TxInput> inputs,
            List<TxOutput> outputs,
            UInt32 lockTime,
            UInt256? hash = null)
        {
            this.Version = version;
            this.Inputs = inputs;
            this.Outputs = outputs;
            this.LockTime = lockTime;

            var sizeEstimate = inputs.Aggregate(0L, (current, t) => current + t.ScriptSignature.Count);

            sizeEstimate = outputs.Aggregate(sizeEstimate, (current, t) => current + t.ScriptPublicKey.Count);
            sizeEstimate = (long)(sizeEstimate * 1.5);
            this.SizeEstimate = sizeEstimate;

            this.Hash = hash ?? DataCalculator.CalculateTransactionHash(version, inputs, outputs, lockTime);
        }

        public UInt32 Version { get; private set; }

        public List<TxInput> Inputs { get; private set; }

        public List<TxOutput> Outputs { get; private set; }

        public UInt32 LockTime { get; private set; }

        public UInt256 Hash { get; private set; }

        public long SizeEstimate { get; private set; }
    }
}