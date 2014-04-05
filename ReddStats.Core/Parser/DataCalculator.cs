namespace ReddStats.Core.Parser
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;

    using bsparser;

    using ReddStats.Core;

    public static class DataCalculator
    {
        private static readonly ThreadLocal<SHA256Managed> _sha256 = new ThreadLocal<SHA256Managed>();

        public static byte[] DoubleSHA256(byte[] input, int offset, int length)
        {
            var sha256 = new SHA256Managed();
            var hash = sha256.ComputeHash(sha256.ComputeHash(input, offset, length));
            return hash;
        }

        public static string GetToAddress(byte[] scriptBytes)
        {
            var add = new Script(scriptBytes, 0, scriptBytes.Length).GetToAddress();
            var str = add.ToString();
            return str;
        }

        public static String BytesToHexString(byte[] bytes)
        {
            StringBuilder buf = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
            {
                String s = (0xFF & b).ToString("x");//  Integer.toString(0xFF & b, 16);    // TODO: correct?
                if (s.Length < 2)
                {
                    buf.Append('0');
                }
                buf.Append(s);
            }
            return buf.ToString();
        }

        private static SHA256Managed SHA256
        {
            get
            {
                return _sha256.IsValueCreated ? _sha256.Value : new SHA256Managed();
            }
        }

        public static byte[] DoubleSHA256(byte[] buffer)
        {
            return SHA256.ComputeHash(SHA256.ComputeHash(buffer));
        }

        public static byte[] CalculateTransactionHash(
            UInt32 version,
            List<TransactionInput> inputs,
            List<TransactionOutput> outputs,
            UInt32 lockTime)
        {
            return DoubleSHA256(EncodeTransaction(version, inputs, outputs, lockTime));
        }

        public static byte[] EncodeTransaction(
            UInt32 version,
            List<TransactionInput> inputs,
            List<TransactionOutput> outputs,
            UInt32 lockTime)
        {
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write4Bytes(version);
                writer.WriteVarInt((UInt64)inputs.Count);
                foreach (var input in inputs)
                {
                    writer.Write(input.PreviousTxOutputKeyBinary);
                    writer.Write4Bytes((uint)input.PreviousOutputIndex);
                    writer.WriteVarBytes(input.ScriptSignature);
                    writer.Write4Bytes((uint)input.Sequence);
                }
                writer.WriteVarInt((UInt64)outputs.Count);
                foreach (var output in outputs)
                {
                    writer.Write8Bytes(Convert.ToUInt64(output.Amount *  BlockParser.Divide));
                    writer.WriteVarBytes(output.ScriptPublicKey);
                }
                writer.Write4Bytes(lockTime);

                return stream.ToArray();
            }
        }
    }
}