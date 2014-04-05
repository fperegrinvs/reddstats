namespace ReddStats.Practices
{
    using System;

    using Lesula.Cassandra;

    /// <summary>
    /// Extensões para uso do LZ4
    /// </summary>
    public unsafe static class LZ4Extensions
    {
        /// <summary>
        /// Comprime um array de bytes
        /// </summary>
        /// <param name="source">bytes a serem comprimidos</param>
        /// <returns>array de bytes com o resultado da compressão</returns>
        public static byte[] CompressLZ4(this byte[] source)
        {
            var bytes = LZ4Sharp.LZ4.Compress(source);
            var result = new byte[bytes.Length + 4];
            var size = BitConverter.GetBytes(source.Length);
            Array.Copy(size, result, 4);
            Array.Copy(bytes, 0, result, 4, bytes.Length);
            return result;
        }

        public static byte[] DecompressLZ4(this byte[] source)
        {
            var size = BitConverter.ToInt32(source, 0);
            var target = new byte[size];

            fixed (byte* t = target, s = source)
            {
                LZ4Sharp.LZ4.DecompressKnownSize(s + 4, t, size);
            }

            return target;
        }

        /// <summary>
        /// Converte uma string para um array de bytes comprimido
        /// </summary>
        /// <param name="text">texto a ser comprimido</param>
        /// <returns>array de bytes com o texto comprimido</returns>
        public static byte[] CompressLZ4(this string text)
        {
            return CompressLZ4(text.ToBytes());
        }
    }
}
