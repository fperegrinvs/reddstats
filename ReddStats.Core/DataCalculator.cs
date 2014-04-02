namespace bsparser
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;

    using ReddStats.Core;

    public static class DataCalculator
    {
        private static readonly ThreadLocal<SHA256Managed> _sha256 = new ThreadLocal<SHA256Managed>();

        /**
* Calculates the SHA-256 hash of the given byte range, and then hashes the resulting hash again. This is
* standard procedure in BitCoin. The resulting hash is in big endian form.
*/
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
            UInt32 Version,
            List<TxInput> Inputs,
            List<TxOutput> Outputs,
            UInt32 LockTime)
        {
            return DoubleSHA256(EncodeTransaction(Version, Inputs, Outputs, LockTime));
        }

        public static byte[] EncodeTransaction(
            UInt32 Version,
            List<TxInput> Inputs,
            List<TxOutput> Outputs,
            UInt32 LockTime)
        {
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write4Bytes(Version);
                writer.WriteVarInt((UInt64)Inputs.Count);
                foreach (var input in Inputs)
                {
                    writer.Write(input.PreviousTxOutputKeyBinary);
                    writer.Write4Bytes(input.PreviousOutputIndex);
                    writer.WriteVarBytes(input.ScriptSignature);
                    writer.Write4Bytes(input.Sequence);
                }
                writer.WriteVarInt((UInt64)Outputs.Count);
                foreach (var output in Outputs)
                {
                    writer.Write8Bytes(Convert.ToUInt64(output.Value *  BlockParser.Divide));
                    writer.WriteVarBytes(output.ScriptPublicKey);
                }
                writer.Write4Bytes(LockTime);

                return stream.ToArray();
            }
        }
    }
}