using System;
using System.Text;

namespace bsparser
{
    public class Base58
    {
        private const string ALPHABET = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

        private static byte Divmod58(byte[] number, int startAt)
        {
            int remainder = 0;
            for (int i = startAt; i < number.Length; i++)
            {
                int digit256 = number[i] & 0xFF;
                int temp = remainder * 256 + digit256;

                number[i] = (byte)(temp / 58);

                remainder = temp % 58;
            }

            return (byte)remainder;
        }

        public static string Encode(byte[] input)   // TODO: replace with BigInteger member?
        {
            int zeroCount = 0;
            while (zeroCount < input.Length && input[zeroCount] == 0)
            {
                ++zeroCount;
            }

            var temp = new byte[input.Length * 2];
            int j = temp.Length;

            int startAt = zeroCount;
            while (startAt < input.Length)
            {
                byte mod = Divmod58(input, startAt);
                if (input[startAt] == 0)
                {
                    ++startAt;
                }
                temp[--j] = (byte)ALPHABET[mod];
            }

            // Strip extra '1' if there are some after decoding.
            while (j < temp.Length && temp[j] == ALPHABET[0])
            {
                ++j;
            }

            // Add as many leading '1' as there were leading zeros.
            while (--zeroCount >= 0)
            {
                temp[--j] = (byte)ALPHABET[0];
            }

            byte[] output = new byte[temp.Length - j];
            Array.Copy(temp, j, output, 0, temp.Length - j);

            var r = Encoding.ASCII.GetString(output);
            return r;
        }
    }
}
