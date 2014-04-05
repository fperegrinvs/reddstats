namespace ReddStats.Core.Parser
{
    using System;
    using System.IO;

    public static class WriterExtensionMethods
    {
        public static void Write1Byte(this BinaryWriter writer, Byte value)
        {
            writer.Write(value);
        }

        public static void Write2Bytes(this BinaryWriter writer, UInt16 value)
        {
            writer.Write(value);
        }


        public static void Write4Bytes(this BinaryWriter writer, UInt32 value)
        {
            writer.Write(value);
        }

        public static void Write8Bytes(this BinaryWriter writer, UInt64 value)
        {
            writer.Write(value);
        }

        public static void Write32Bytes(this BinaryWriter writer, UInt256 value)
        {
            writer.Write(value.ToByteArray());
        }

        public static void WriteBytes(this BinaryWriter writer, byte[] value)
        {
            writer.Write(value);
        }

        public static void WriteBytes(this BinaryWriter writer, int length, byte[] value)
        {
            if (value.Length != length)
                throw new ArgumentException();

            writer.WriteBytes(value);
        }

        public static void WriteVarInt(this BinaryWriter writer, UInt64 value)
        {
            if (value < 0xFD)
            {
                writer.Write1Byte((Byte)value);
            }
            else if (value <= 0xFFFF)
            {
                writer.Write1Byte(0xFD);
                writer.Write2Bytes((UInt16)value);
            }
            else if (value <= 0xFFFFFFFF)
            {
                writer.Write1Byte(0xFE);
                writer.Write4Bytes((UInt32)value);
            }
            else
            {
                writer.Write1Byte(0xFF);
                writer.Write8Bytes(value);
            }
        }

        public static void WriteVarBytes(this BinaryWriter writer, byte[] value)
        {
            writer.WriteVarInt((UInt64)value.Length);
            writer.WriteBytes(value.Length, value);
        }

        public static void WriteVarBytes(this BinaryWriter writer, string value)
        {
            var bytes = value.FromHexStringReverse();

            writer.WriteVarInt((UInt64)bytes.Length);
            writer.WriteBytes(bytes.Length, bytes);
        }
    }
}