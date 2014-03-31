namespace bsparser
{
    using System;

    public class BlockHeader
    {
        private readonly int hashCode;

        public BlockHeader(
            UInt32 version,
            UInt256 previousBlock,
            UInt256 merkleRoot,
            UInt32 time,
            UInt32 bits,
            UInt32 nonce,
            UInt256? hash = null)
        {
            this.Version = version;
            this.PreviousBlock = previousBlock;
            this.MerkleRoot = merkleRoot;
            this.Time = time;
            this.Bits = bits;
            this.Nonce = nonce;

            this.Hash = hash ?? new UInt256();

            this.hashCode = this.Hash.GetHashCode();
        }

        public UInt32 Version { get; private set; }

        public UInt256 PreviousBlock { get; private set; }

        public UInt256 MerkleRoot { get; private set; }

        public UInt32 Time { get; private set; }

        public UInt32 Bits { get; private set; }

        public UInt32 Nonce { get; private set; }

        public UInt256 Hash { get; private set; }

        public override int GetHashCode()
        {
            return this.hashCode;
        }
    }
}