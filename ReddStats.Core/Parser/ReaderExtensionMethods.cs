namespace ReddStats.Core.Parser
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public static class ReaderExtensionMethods
    {
        public static List<T> DecodeList<T>(this BinaryReader reader, Func<T> decode)
        {
            var length = reader.ReadVarInt().ToIntChecked();

            var list = new T[length];
            for (var i = 0; i < length; i++)
            {
                list[i] = decode();
            }

            return list.ToList();//ToImmutableArray();
        }

        public static UInt16 Read2Bytes(this BinaryReader reader)
        {
            return reader.ReadUInt16();
        }

        public static UInt32 Read4Bytes(this BinaryReader reader)
        {
            return reader.ReadUInt32();
        }

        public static UInt64 Read8Bytes(this BinaryReader reader)
        {
            return reader.ReadUInt64();
        }

        public static UInt256 Read32Bytes(this BinaryReader reader)
        {
            return new UInt256(reader.ReadBytes(32));
        }

        public static string Read32BytesString(this BinaryReader reader)
        {
            return reader.ReadBytes(32).ToHexStringReverse();
        }
        public static UInt64 ReadVarInt(this BinaryReader reader)
        {
            var value = reader.ReadByte();
            if (value < 0xFD)
                return value;
            if (value == 0xFD)
                return reader.Read2Bytes();
            if (value == 0xFE)
                return reader.Read4Bytes();
            if (value == 0xFF)
                return reader.Read8Bytes();

            throw new Exception();
        }

        public static byte[] ReadVarBytes(this BinaryReader reader)
        {
            var length = reader.ReadVarInt();
            return reader.ReadBytes(length.ToIntChecked());
        }
    }
}