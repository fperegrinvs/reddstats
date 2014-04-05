namespace ReddStats.Core.Parser
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class Script
    {
        //private static Logger log = LoggerFactory.getLogger(Script.class);

        // Some constants used for decoding the scripts.
        public const int OP_PUSHDATA1 = 76;
        public const int OP_PUSHDATA2 = 77;
        public const int OP_PUSHDATA4 = 78;
        public const int OP_DUP = 118;
        public const int OP_HASH160 = 169;
        public const int OP_EQUALVERIFY = 136;
        public const int OP_CHECKSIG = 172;
        public const uint PacketMagic = 0xdbb6c0fb;

        /** First byte of a base58 encoded address. See {@link Address}*/
        public const int addressHeader = 61;

        byte[] program;
        private int cursor;

        // The program is a set of byte[]s where each element is either [opcode] or [data, data, data ...]
        private List<byte[]> chunks;


        /**
         * Construct a Script using the given network parameters and a range of the programBytes array.
         * @param params Network parameters.
         * @param programBytes Array of program bytes from a transaction.
         * @param offset How many bytes into programBytes to start reading from.
         * @param length How many bytes to read.
         * @throws ScriptException
         */
        public Script(byte[] programBytes, int offset, int length)
        {
            this.parse(programBytes, offset, length);
        }



        /** Returns the program opcodes as a string, for example "[1234] DUP HAHS160" */
        public override string ToString()
        {
            StringBuilder buf = new StringBuilder();
            foreach (byte[] chunk in this.chunks)
            {
                if (chunk.Length == 1)
                {
                    String opName;
                    int opcode = 0xFF & chunk[0];
                    switch (opcode)
                    {
                        case OP_DUP:
                            opName = "DUP";
                            break;
                        case OP_HASH160:
                            opName = "HASH160";
                            break;
                        case OP_CHECKSIG:
                            opName = "CHECKSIG";
                            break;
                        case OP_EQUALVERIFY:
                            opName = "EQUALVERIFY";
                            break;
                        default:
                            opName = "?(" + opcode + ")";
                            break;
                    }
                    buf.Append(opName);
                    buf.Append(" ");
                }
                else
                {
                    // Data chunk
                    buf.Append("[");
                    buf.Append(chunk.Length);
                    buf.Append("]");
                    buf.Append(DataCalculator.BytesToHexString(chunk));
                    buf.Append(" ");
                }
            }
            return buf.ToString();
        }


        private byte[] GetData(int len)
        {
            try
            {
                byte[] buf = new byte[len];
                Array.Copy(this.program, this.cursor, buf, 0, len);
                this.cursor += len;
                return buf;
            }
            catch (Exception e)
            {
                // We want running out of data in the array to be treated as a handleable script parsing exception,
                // not something that abnormally terminates the app.
                throw new Exception("Failed read of " + len + " bytes", e);
            }
        }

        private int readByte()
        {
            return 0xFF & this.program[this.cursor++];
        }

        /**
         * To run a script, first we parse it which breaks it up into chunks representing pushes of
         * data or logical opcodes. Then we can run the parsed chunks.
         *
         * The reason for this split, instead of just interpreting directly, is to make it easier
         * to reach into a programs structure and pull out bits of data without having to run it.
         * This is necessary to render the to/from addresses of transactions in a user interface.
         * The official client does something similar.
         */
        private void parse(byte[] programBytes, int offset, int length)
        {
            this.program = programBytes;
            this.chunks = new List<byte[]>(10);  // Arbitrary choice of initial size.
            this.cursor = offset;
            while (this.cursor < offset + length)
            {
                int opcode = this.readByte();
                if (opcode >= 0xF0)
                {
                    // Not a single byte opcode.
                    opcode = (opcode << 8) | this.readByte();
                }

                if (opcode > 0 && opcode < OP_PUSHDATA1)
                {
                    // Read some bytes of data, where how many is the opcode value itself.
                    this.chunks.Add(this.GetData(opcode));  // opcode == len here.
                }
                else if (opcode == OP_PUSHDATA1)
                {
                    int len = this.readByte();
                    this.chunks.Add(this.GetData(len));
                }
                else if (opcode == OP_PUSHDATA2)
                {
                    // Read a short, then read that many bytes of data.
                    int len = this.readByte() | (this.readByte() << 8);
                    this.chunks.Add(this.GetData(len));
                }
                else if (opcode == OP_PUSHDATA4)
                {
                    // Read a uint32, then read that many bytes of data.
                    // TODO: log.error("PUSHDATA4: Unimplemented");
                }
                else
                {
                    this.chunks.Add(new[] { (byte)opcode });
                }
            }
        }

        /**
         * If a program matches the standard template DUP HASH160 <pubkey hash> EQUALVERIFY CHECKSIG
         * then this function retrieves the third element, otherwise it throws a ScriptException.
         *
         * This is useful for fetching the destination address of a transaction.
         */
        public byte[] GetPubKeyHash()
        {
            if (this.chunks.Count == 2)
            {
                byte[] hash = new byte[20];
                Array.Copy(this.chunks[0], 1, hash, 0, 20);
                return hash;
            }

            if (this.chunks.Count != 5)
                throw new Exception("Script not of right size to be a scriptPubKey, " +
                                    "expecting 5 but got " + this.chunks.Count);
            if ((0xFF & this.chunks[0][0]) != OP_DUP ||
                (0xFF & this.chunks[1][0]) != OP_HASH160 ||
                (0xFF & this.chunks[3][0]) != OP_EQUALVERIFY ||
                (0xFF & this.chunks[4][0]) != OP_CHECKSIG)
                throw new Exception("Script not in the standard scriptPubKey form");

            // Otherwise, the third element is the hash of the public key, ie the bitcoin address.
            return this.chunks[2];
        }

        /**
         * Gets the destination address from this script, if it's in the required form (see getPubKey).
         * @throws ScriptException
         */
        public VersionedChecksummedBytes GetToAddress()
        {
            return new VersionedChecksummedBytes(addressHeader, this.GetPubKeyHash());
        }

    }
}