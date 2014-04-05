namespace ReddStats.Core.VO
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ProtoBuf;

    using ReddStats.Core.Parser;

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
                inputs[k].TransactionId = this.TransactionId;
                inputs[k].BlockId = this.BlockId;
                this.TotalIn += inputs[k].Amount;
            }

            for (var k = 0; k < outputs.Count; k++)
            {
                outputs[k].Index = k;
                outputs[k].TransactionId = this.TransactionId;
                outputs[k].BlockId = this.BlockId;
                this.TotalOut += outputs[k].Amount;
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
                return this.Inputs.Count;
            }
        }

        public int OutputsCount
        {
            get
            {
                return this.Outputs.Count;
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
                return this.TotalIn == 0 ? 0M : this.TotalIn - this.TotalOut;
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
                    this.transactionId = DataCalculator.CalculateTransactionHash((uint)this.Version, this.Inputs, this.Outputs, (uint)this.LockTime).ToHexStringReverse();
                }

                return this.transactionId;
            }
            set
            {
                this.transactionId = value;
            }
        }

        [ProtoMember(10)]

        public long Size { get; private set; }
    }
}