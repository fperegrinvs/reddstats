namespace bsparser
{
    using System;

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
                CharLookupTable[i] = i.ToString("x2").ToUpper().ToCharArray();
            }
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
        public static string ToHexStringReverse(this byte[] bytes)
        {
            var u = bytes.Length;
            var chars = new char[u * 2];
            int offset = u - 1;

            int b = 0;
            while (offset >= 0)
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