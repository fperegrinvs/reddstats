namespace ReddStats.Core.VO
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    using ProtoBuf;

    using ReddStats.Core.Parser;

    [ProtoContract]
    [DataContract]
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

        [DataMember]
        [ProtoMember(1)]
        public int BlockId { get; set; }

        [DataMember]
        public int InputsCount
        {
            get
            {
                return this.Inputs.Count;
            }
        }

        [DataMember]
        public int OutputsCount
        {
            get
            {
                return this.Outputs.Count;
            }
        }

        [ProtoMember(2)]
        [DataMember]
        public decimal TotalIn { get; set; }


        [ProtoMember(3)]
        [DataMember]
        public decimal TotalOut { get; private set; }

        [DataMember]
        public decimal Fee
        {
            get
            {
                return this.TotalIn == 0 ? 0M : this.TotalIn - this.TotalOut;
            }
        }

        [DataMember]
        [ProtoMember(4)]
        public DateTime? Date { get; set; }


        [DataMember]
        [ProtoMember(5)]
        public long Version { get; set; }


        [DataMember]
        [ProtoMember(6)]
        public List<TransactionInput> Inputs { get; set; }

        [DataMember]
        [ProtoMember(7)]
        public List<TransactionOutput> Outputs { get; set; }

        [DataMember]
        [ProtoMember(8)]
        public long LockTime { get; set; }

        private string transactionId;

        [DataMember]
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

        [DataMember]
        [ProtoMember(10)]
        public long Size { get; private set; }
    }
}