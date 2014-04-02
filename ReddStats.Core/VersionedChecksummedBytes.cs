namespace bsparser
{
    using System;
    using System.Diagnostics;

    using ReddStats.Core;

    public class VersionedChecksummedBytes
    {
        protected int Version;
        protected readonly byte[] Bytes;

        public VersionedChecksummedBytes(int version, byte[] bytes)
        {
            Debug.Assert(version < 256 && version >= 0);
            this.Version = version;
            this.Bytes = bytes;
        }

        public override string ToString()
        {
            // A stringified address is:
            //   1 byte version + 20 bytes hash + 4 bytes check code (itself a truncated hash)
            var addressBytes = new byte[1 + this.Bytes.Length + 4];
            addressBytes[0] = (byte)this.Version;
            Array.Copy(this.Bytes, 0, addressBytes, 1, this.Bytes.Length);
            byte[] check = DataCalculator.DoubleSHA256(addressBytes, 0, this.Bytes.Length + 1);
            Array.Copy(check, 0, addressBytes, this.Bytes.Length + 1, 4);
            return Base58.Encode(addressBytes);
        }

        public override int GetHashCode()
        {
            return this.Bytes.GetHashCode();
        }

        public override bool Equals(object o)
        {
            if (!(o is VersionedChecksummedBytes)) return false;
            var vcb = (VersionedChecksummedBytes)o;
            return Equals(vcb.Bytes, this.Bytes);
        }
    }
}