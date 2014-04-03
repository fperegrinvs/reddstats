namespace ReddStats.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Block
    {
        public List<Transaction> Transactions { get; set; }

        private readonly int hashCode;

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

            this.hashCode = this.Hash.GetHashCode();
        }

        public long Version { get; private set; }

        public decimal CoinsCreated
        {
            get
            {
                return this.Transactions[0].Outputs[0].Amount;
            }
        }

        public string PreviousBlockHash { get; private set; }

        public string MerkleRoot { get; private set; }

        public DateTime Date { get; private set; }

        public double Difficulty { get; private set; }

        public long Nonce { get; private set; }

        public string Hash { get; set; }

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

        public int Id { get; set; }

        public override int GetHashCode()
        {
            return this.hashCode;
        }

    }
}