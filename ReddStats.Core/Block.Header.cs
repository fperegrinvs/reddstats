namespace bsparser
{
    using System;

    public partial class Block
    {
        private readonly int hashCode;

        public Block(
            UInt32 version,
            string previousBlockHash,
            string merkleRoot,
            DateTime date,
            double bits,
            UInt32 nonce,
            string hash = null)
        {
            this.Version = version;
            this.PreviousBlockHash = previousBlockHash;
            this.MerkleRoot = merkleRoot;
            this.Date = date;
            this.Bits = bits;
            this.Nonce = nonce;

            this.Hash = hash ?? "";

            this.hashCode = this.Hash.GetHashCode();
        }

        public UInt32 Version { get; private set; }

        public string PreviousBlockHash { get; private set; }

        public string MerkleRoot { get; private set; }

        public DateTime Date { get; private set; }

        public double Bits { get; private set; }

        public UInt32 Nonce { get; private set; }

        public string Hash { get; set; }

        public string NextBlockHash { get; set; }

        public int BlockId { get; set; }

        public override int GetHashCode()
        {
            return this.hashCode;
        }
    }
}