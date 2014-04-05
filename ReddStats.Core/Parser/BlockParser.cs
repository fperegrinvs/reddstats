namespace ReddStats.Core.Parser
{
    using System;
    using System.IO;
    using System.Text;

    using ReddStats.Core.Interface;
    using ReddStats.Core.VO;

    public static class BlockParser
    {
        public const decimal Divide = 100000000M;

        public static Block ReadBlock(Stream stream, int blockHeight, IBlockChainDataProvider provider)
        {
            using (var reader = new BinaryReader(stream, Encoding.ASCII, true))
            {
                if (reader.PeekChar() == -1)
                {
                    return null;
                }

                var block = DecodeBlockHeader(stream, blockHeight);

                if (block == null)
                {
                    return null;
                }

                var transactions = reader.DecodeList(() => DecodeTransaction(stream, blockHeight, provider));

                block.Transactions = transactions;

                return block;
            }
        }

        private static Block DecodeBlockHeader(Stream stream, int blockHeight)
        {
            using (var reader = new BinaryReader(stream, Encoding.ASCII, true))
            {
                var magic = reader.Read4Bytes();
                if (magic == Script.PacketMagic)
                {
                    reader.Read4Bytes(); // size
                }
                else
                {
                    stream.Position -= 4;
                }

                var headerBin = reader.ReadBytes(80);

                var version = BitConverter.ToUInt32(headerBin, 0); // reader.Read4Bytes();

                if (version == 0)
                {
                    return null;
                }

                var previousBlock = headerBin.ToHexStringReverse(4, 32); //reader.Read32BytesString();
                var merkleRoot = headerBin.ToHexStringReverse(36, 32);// reader.Read32BytesString();
                var time = BitConverter.ToUInt32(headerBin, 68).ToDateTime(); // reader.Read4Bytes().ToDateTime();
                var bits = BitConverter.ToUInt32(headerBin, 72).GetDifficulty();// reader.Read4Bytes().GetDifficulty();
                var nonce = BitConverter.ToUInt32(headerBin, 76);// reader.Read4Bytes();
                return new Block(version, previousBlock, merkleRoot, time, bits, nonce) { Id = blockHeight };
            }
        }

        private static TransactionInput DecodeTxInput(Stream stream, IBlockChainDataProvider transactionProvider)
        {
            using (var reader = new BinaryReader(stream, Encoding.ASCII, true))
            {
                var keyBytes = reader.ReadBytes(32);
                var txInput = new TransactionInput
                       {
                           PreviousOutputKey = keyBytes.ToHexStringReverse(),
                           PreviousTxOutputKeyBinary = keyBytes,
                           PreviousOutputIndex = (int)reader.Read4Bytes(),
                           ScriptSignature = reader.ReadVarBytes().ToHexStringReverse(),
                           Sequence = reader.Read4Bytes()
                       };

                var origin = transactionProvider.GetTransactionOutput(
                    txInput.PreviousOutputKey,
                    txInput.PreviousOutputIndex);

                if (origin != null)
                {
                    txInput.Amount = origin.Amount;
                    txInput.FromAddress = origin.ToAddress;
                }

                return txInput;
            }
        }

        private static TransactionOutput DecodeTxOutput(Stream stream)
        {
            using (var reader = new BinaryReader(stream, Encoding.ASCII, true))
            {
                var value = reader.Read8Bytes();
                var bytes = reader.ReadVarBytes();
                return new TransactionOutput
                {
                    Amount = value/ Divide,
                    ScriptPublicKeyBinary = bytes,
                    ScriptPublicKey = bytes.ToHexStringReverse()
                };
            }
        }

        private static Transaction DecodeTransaction(Stream stream, int blockId, IBlockChainDataProvider transactionProvider)
        {
            using (var reader = new BinaryReader(stream, Encoding.ASCII, true))
            {
                var transaction = new Transaction(
                    reader.Read4Bytes(),
                    reader.DecodeList(() => DecodeTxInput(stream, transactionProvider)),
                    reader.DecodeList(() => DecodeTxOutput(stream)),
                    reader.Read4Bytes(), blockId);

                transactionProvider.SaveTransaction(transaction);
                return transaction;
            }
        }
    }
}

