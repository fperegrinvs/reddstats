namespace bsparser
{
    using System;
    using System.Globalization;

    using Org.BouncyCastle.Math;

    public static class ExtensionMethods
    {
        /// <summary>
        /// Tabela para codificação de caracteres
        /// </summary>
        private static readonly char[][] CharLookupTable;

        /// <summary>
        /// Initializes static members of the <see cref="ExtensionMethods"/> class.
        /// </summary>
        static ExtensionMethods()
        {
            CharLookupTable = new char[256][];
            for (var i = 0; i < 256; i++)
            {
                CharLookupTable[i] = i.ToString("x2").ToLowerInvariant().ToCharArray();
            }
        }

        private static readonly byte[] LookupTableLow = new byte[] 
            {
              0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 
              0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 
              0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 
              0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 
              0xff, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 
              0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 
              0xff, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 
              0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff
            };

        private static readonly byte[] LookupTableHigh = new byte[] 
            {
              0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 
              0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 
              0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 
              0x00, 0x10, 0x20, 0x30, 0x40, 0x50, 0x60, 0x70, 0x80, 0x90, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 
              0xff, 0xa0, 0xb0, 0xc0, 0xd0, 0xe0, 0xf0, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 
              0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 
              0xff, 0xa0, 0xb0, 0xc0, 0xd0, 0xe0, 0xf0, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 
              0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff
           };

        public static byte[] FromHexString(this string s)
        {
            if (s == null)
            {
                throw new ArgumentNullException("s");
            }

            var chars = s.ToCharArray();
            var u = chars.Length;
            var bytes = new byte[u / 2];
            int offset = 0;

            int b = 0;
            while (offset < u)
            {
                bytes[b++] = (byte)(LookupTableHigh[chars[offset++]] | LookupTableLow[chars[offset]]);
                ++offset;
            }

            return bytes;
        }

        public static byte[] FromHexStringReverse(this string s)
        {
            if (s == null)
            {
                throw new ArgumentNullException("s");
            }

            var chars = s.ToCharArray();
            var u = chars.Length;
            var bytes = new byte[u / 2];
            int offset = chars.Length - 1;

            int b = 0;
            while (offset > 0)
            {
                bytes[b++] = (byte)(LookupTableHigh[chars[offset-1]] | LookupTableLow[chars[offset]]);
                offset -= 2;
            }

            return bytes;
        }

        public static UInt256 HighestTarget = UInt256.Parse("00000000FFFF0000000000000000000000000000000000000000000000000000", NumberStyles.HexNumber);

        public static double TargetToDifficulty(UInt256 target)
        {
            // difficulty is HighestTarget / target
            // since these are 256-bit numbers, use division trick for BigIntegers
            return Math.Exp(System.Numerics.BigInteger.Log(HighestTarget.ToBigInteger()) - System.Numerics.BigInteger.Log(target.ToBigInteger()));
        }


        public static UInt256 BitsToTarget(UInt32 bits)
        {
            // last three bytes store the multiplicand
            var multiplicand = (UInt256)bits % 0x1000000;
            if (multiplicand > 0x7fffff)
                throw new ArgumentOutOfRangeException("bits");

            // first byte stores the value to be used in the power
            var powerPart = (int)(bits >> 24);
            var multiplier = new UInt256(System.Numerics.BigInteger.Pow(2, 8 * (powerPart - 3)));

            return multiplicand * multiplier;
        }

        public static double GetDifficulty(this uint bits)
        {
            var target = BitsToTarget(bits);
            var diff = TargetToDifficulty(target);
            return diff;
        }

        /// <summary>
        /// Converte um bytearray para uma string de hexadecimais
        /// </summary>
        /// <param name="bytes">
        /// O array de bytes a ser convertido
        /// </param>
        /// <returns>
        /// string com a representação hexa do array.
        /// </returns>
        public static string ToHexStringReverse(this byte[] bytes, int start = 0, int len = -1)
        {
            var u = len == -1 ? bytes.Length : start + len;
            var chars = new char[(u-start) * 2];
            int offset = u - 1;

            int b = 0;
            while (offset >= start)
            {
                var ch = CharLookupTable[bytes[offset]];
                chars[b++] = ch[0];
                chars[b++] = ch[1];
                --offset;
            }

            return new string(chars);
        }

        public static byte[] Concat(this byte[] first, byte[] second)
        {
            var buffer = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, buffer, 0, first.Length);
            Buffer.BlockCopy(second, 0, buffer, first.Length, second.Length);
            return buffer;
        }

        public static byte[] Concat(this byte[] first, byte second)
        {
            var buffer = new byte[first.Length + 1];
            Buffer.BlockCopy(first, 0, buffer, 0, first.Length);
            buffer[buffer.Length - 1] = second;
            return buffer;
        }

        public static DateTime ToDateTime(this uint value)
        {
            // Unix timestamp is seconds past epoch
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(value);
            return dtDateTime;
        }

        public static string ToHexNumberString(this byte[] value)
        {
            return "0x" + value.ToHexStringReverse();
        }

        public static string ToHexNumberString(this UInt256 value)
        {
            return ToHexNumberString(value.ToByteArray());
        }


        public static int ToIntChecked(this UInt64 value)
        {
            checked
            {
                return (int)value;
            }
        }
    }
}