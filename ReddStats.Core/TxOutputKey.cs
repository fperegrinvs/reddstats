namespace bsparser
{
    using System;

    public class TxOutputKey
    {
        private readonly int hashCode;

        public TxOutputKey(UInt256 txHash, UInt32 txOutputIndex)
        {
            this.TxHash = txHash;
            this.TxOutputIndex = txOutputIndex;

            this.hashCode = txHash.GetHashCode() ^ txOutputIndex.GetHashCode();
        }

        public UInt256 TxHash { get; private set; }

        public UInt32 TxOutputIndex { get; private set; }

        public override bool Equals(object obj)
        {
            if (!(obj is TxOutputKey)) return false;

            return (TxOutputKey)obj == this;
        }

        public override int GetHashCode()
        {
            return this.hashCode;
        }

        public static bool operator ==(TxOutputKey left, TxOutputKey right)
        {
            return ReferenceEquals(left, right)
                   || (!ReferenceEquals(left, null) && !ReferenceEquals(right, null)
                       && left.TxHash == right.TxHash && left.TxOutputIndex == right.TxOutputIndex);
        }

        public static bool operator !=(TxOutputKey left, TxOutputKey right)
        {
            return !(left == right);
        }
    }
}