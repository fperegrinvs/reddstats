namespace bsparser
{
    using System;
    using System.Globalization;

    public struct UInt256 
    {
        // parts are big-endian
        private readonly UInt64 part1;
        private readonly UInt64 part2;
        private readonly UInt64 part3;
        private readonly UInt64 part4;
        private readonly int hashCode;

        public UInt256(byte[] value)
        {
            if (value.Length > 32 && !(value.Length == 33 && value[32] == 0))
                throw new ArgumentOutOfRangeException();

            if (value.Length < 32)
                value = value.Concat(new byte[32 - value.Length]);

            // read LE parts in reverse order to store in BE
            var part1Bytes = new byte[8];
            var part2Bytes = new byte[8];
            var part3Bytes = new byte[8];
            var part4Bytes = new byte[8];
            Buffer.BlockCopy(value, 0, part4Bytes, 0, 8);
            Buffer.BlockCopy(value, 8, part3Bytes, 0, 8);
            Buffer.BlockCopy(value, 16, part2Bytes, 0, 8);
            Buffer.BlockCopy(value, 24, part1Bytes, 0, 8);

            // convert parts and store
            this.part1 = BitConverter.ToUInt64(part1Bytes, 0);
            this.part2 = BitConverter.ToUInt64(part2Bytes, 0);
            this.part3 = BitConverter.ToUInt64(part3Bytes, 0);
            this.part4 = BitConverter.ToUInt64(part4Bytes, 0);

            this.hashCode = this.part1.GetHashCode() ^ this.part2.GetHashCode() ^ this.part3.GetHashCode() ^ this.part4.GetHashCode();
        }

        public UInt256(int value)
            : this(BitConverter.GetBytes(value))
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException();
        }

        public UInt256(long value)
            : this(BitConverter.GetBytes(value))
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException();
        }

        public UInt256(uint value)
            : this(BitConverter.GetBytes(value))
        { }

        public UInt256(ulong value)
            : this(BitConverter.GetBytes(value))
        { }

        public UInt256(System.Numerics.BigInteger value)
            : this(value.ToByteArray())
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException();
        }

        public System.Numerics.BigInteger ToBigInteger()
        {
            // add a trailing zero so that value is always positive
            return new System.Numerics.BigInteger(ToByteArray().Concat(0));
        }


        public static UInt256 operator *(UInt256 left, UInt256 right)
        {
            return new UInt256(left.ToBigInteger() * right.ToBigInteger());
        }

        public static UInt256 Parse(string value, NumberStyles style)
        {
            return new UInt256(System.Numerics.BigInteger.Parse("0" + value, style).ToByteArray());
        }

        public static UInt256 Parse(string value, IFormatProvider provider)
        {
            return new UInt256(System.Numerics.BigInteger.Parse("0" + value, provider).ToByteArray());
        }

        public byte[] ToByteArray()
        {
            var buffer = new byte[32];
            Buffer.BlockCopy(BitConverter.GetBytes(this.part4), 0, buffer, 0, 8);
            Buffer.BlockCopy(BitConverter.GetBytes(this.part3), 0, buffer, 8, 8);
            Buffer.BlockCopy(BitConverter.GetBytes(this.part2), 0, buffer, 16, 8);
            Buffer.BlockCopy(BitConverter.GetBytes(this.part1), 0, buffer, 24, 8);

            return buffer;
        }

        public static implicit operator UInt256(byte value)
        {
            return new UInt256(value);
        }

        public static implicit operator UInt256(int value)
        {
            return new UInt256(value);
        }

        public static implicit operator UInt256(long value)
        {
            return new UInt256(value);
        }

        public static implicit operator UInt256(sbyte value)
        {
            return new UInt256(value);
        }

        public static implicit operator UInt256(short value)
        {
            return new UInt256(value);
        }

        public static implicit operator UInt256(uint value)
        {
            return new UInt256(value);
        }

        public static implicit operator UInt256(ulong value)
        {
            return new UInt256(value);
        }

        public static implicit operator UInt256(ushort value)
        {
            return new UInt256(value);
        }

        public static bool operator ==(UInt256 left, UInt256 right)
        {
            return /*object.ReferenceEquals(left, right) || (!object.ReferenceEquals(left, null) && !object.ReferenceEquals(right, null) &&*/ left.part1 == right.part1 && left.part2 == right.part2 && left.part3 == right.part3 && left.part4 == right.part4/*)*/;
        }

        public static UInt256 operator %(UInt256 dividend, UInt256 divisor)
        {
            return new UInt256(dividend.ToBigInteger() % divisor.ToBigInteger());
        }

        public static bool operator !=(UInt256 left, UInt256 right)
        {
            return !(left == right);
        }

        public static bool operator <(UInt256 left, UInt256 right)
        {
            if (left.part1 < right.part1)
                return true;
            if (left.part1 == right.part1 && left.part2 < right.part2)
                return true;
            if (left.part1 == right.part1 && left.part2 == right.part2 && left.part3 < right.part3)
                return true;
            if (left.part1 == right.part1 && left.part2 == right.part2 && left.part3 == right.part3 && left.part4 < right.part4)
                return true;

            return false;
        }


        public static bool operator >(UInt256 left, UInt256 right)
        {
            if (left.part1 > right.part1)
                return true;
            if (left.part1 == right.part1 && left.part2 > right.part2)
                return true;
            if (left.part1 == right.part1 && left.part2 == right.part2 && left.part3 > right.part3)
                return true;
            if (left.part1 == right.part1 && left.part2 == right.part2 && left.part3 == right.part3 && left.part4 > right.part4)
                return true;

            return false;
        }

        // TODO doesn't compare against other numerics
        public override bool Equals(object obj)
        {
            if (!(obj is UInt256))
                return false;

            var other = (UInt256)obj;
            return other.part1 == this.part1 && other.part2 == this.part2 && other.part3 == this.part3 && other.part4 == this.part4;
        }

        public override int GetHashCode()
        {
            return this.hashCode;
        }

        public override string ToString()
        {
            return this.ToHexNumberString();
        }
    }
}