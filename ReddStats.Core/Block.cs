namespace ReddStats.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ProtoBuf;

    [ProtoContract]
    public class Block
    {

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

        [ProtoMember(2)]
        public long Version { get; private set; }


        public decimal CoinsCreated
        {
            get
            {
                return this.Transactions[0].Outputs[0].Amount;
            }
        }

        [ProtoMember(3)]
        public string PreviousBlockHash { get; private set; }

        [ProtoMember(4)]
        public string MerkleRoot { get; private set; }

        [ProtoMember(5)]
        public DateTime Date { get; private set; }

        [ProtoMember(6)]
        public double Difficulty { get; private set; }

        [ProtoMember(7)]
        public long Nonce { get; private set; }

        [ProtoMember(8)]
        public string Hash { get; set; }

        [ProtoMember(9)]
        public string NextBlockHash { get; set; }

        public decimal Size
        {
            get
            {
                return Transactions.Sum(t => t.Size);
            }
        }

        public decimal TotalTransactionValue
        {
            get
            {
                return Transactions.Sum(t => t.Outputs.Sum(o => o.Amount));
            }
        }

        public decimal TotalFees
        {
            get
            {
                return Transactions.Sum(t => t.Fee);
            }
        }


        public int TransactionsCount
        {
            get
            {
                return Transactions.Count;
            }
        }

        [ProtoMember(10)]
        public int Id { get; set; }

        [ProtoMember(11)]
        public int HashCode;

        public override int GetHashCode()
        {
            return this.HashCode;
        }

    }
}