namespace ReddStats.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    using ProtoBuf;

    using ReddStats.Core.VO;

    [DataContract]
    [ProtoContract]
    public class Block
    {

        [DataMember]
        [ProtoMember(1)]
        public List<Transaction> Transactions { get; set; }


        public Block(
            long version,
            string previousBlockHash,
            string merkleRoot,
            DateTime date,
            double difficulty,
            long nonce,
            string hash = null)
        {
            this.Version = version;
            this.PreviousBlockHash = previousBlockHash;
            this.MerkleRoot = merkleRoot;
            this.Date = date;
            this.Difficulty = difficulty;
            this.Nonce = nonce;

            this.Hash = hash ?? "";

            this.HashCode = this.Hash.GetHashCode();
        }

        public Block()
        {
            
        }

        [DataMember]
        [ProtoMember(2)]
        public long Version { get; private set; }


        [DataMember]
        public decimal CoinsCreated
        {
            get
            {
                return this.Transactions[0].Outputs[0].Amount;
            }
        }

        [DataMember]
        [ProtoMember(3)]
        public string PreviousBlockHash { get; private set; }

        [DataMember]
        [ProtoMember(4)]
        public string MerkleRoot { get; private set; }

        [DataMember]
        [ProtoMember(5)]
        public DateTime Date { get; private set; }

        [DataMember]
        [ProtoMember(6)]
        public double Difficulty { get; private set; }

        [DataMember]
        [ProtoMember(7)]
        public long Nonce { get; private set; }

        [DataMember]
        [ProtoMember(8)]
        public string Hash { get; set; }

        [DataMember]
        [ProtoMember(9)]
        public string NextBlockHash { get; set; }

        [DataMember]
        public decimal Size
        {
            get
            {
                return Transactions.Sum(t => t.Size);
            }
        }

        [DataMember]
        public decimal TotalTransactionValue
        {
            get
            {
                return Transactions.Sum(t => t.Outputs.Sum(o => o.Amount));
            }
        }

        [DataMember]
        public decimal TotalFees
        {
            get
            {
                return Transactions.Sum(t => t.Fee);
            }
        }

        [DataMember]
        public int TransactionsCount
        {
            get
            {
                return Transactions.Count;
            }
        }

        [DataMember]
        [ProtoMember(10)]
        public int Id { get; set; }

        [DataMember]
        [ProtoMember(11)]
        public int HashCode;

        public override int GetHashCode()
        {
            return this.HashCode;
        }

    }
}