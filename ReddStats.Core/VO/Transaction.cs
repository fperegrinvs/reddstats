namespace ReddStats.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using bsparser;

    using ProtoBuf;

    [ProtoContract]
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

        public Transaction()
        { }

        [ProtoMember(1)]
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

        [ProtoMember(2)]
        public decimal TotalIn { get; set; }


        [ProtoMember(3)]
        public decimal TotalOut { get; private set; }

        public decimal Fee
        {
            get
            {
                return TotalIn == 0 ? 0M : TotalIn - TotalOut;
            }
        }

        [ProtoMember(4)]
        public DateTime? Date { get; set; }


        [ProtoMember(5)]
        public long Version { get; set; }


        [ProtoMember(6)]
        public List<TransactionInput> Inputs { get; set; }

        [ProtoMember(7)]
        public List<TransactionOutput> Outputs { get; set; }

        [ProtoMember(8)]
        public long LockTime { get; set; }

        private string transactionId;

        [ProtoMember(9)]
        public string TransactionId
        {
            get
            {
                if (this.transactionId == null)
                {
                    this.transactionId = DataCalculator.CalculateTransactionHash((uint)Version, Inputs, Outputs, (uint)LockTime).ToHexStringReverse();
                }

                return this.transactionId;
            }
            set
            {
                transactionId = value;
            }
        }

        [ProtoMember(10)]

        public long Size { get; private set; }
    }
}