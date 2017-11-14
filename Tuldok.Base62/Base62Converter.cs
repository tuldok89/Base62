using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Tuldok.Base62
{
    /// <summary>
    /// Encode to Base62.
    /// </summary>
    public class Base62Converter
    {
        static string Codes = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        static char CodeFlag = '9';
        static StringBuilder strout = new StringBuilder();
        static IDictionary<char, int> Codemap = new Dictionary<char, int>();

        static void Append(int b)
        {
            if (b < 61)
            {
                strout.Append(Codes[b]);
            }
            else
            {
                strout.Append(CodeFlag);
                strout.Append(Codes[b - 1]);
            }
        }
        
        /// <summary>
        /// Encode to Base62
        /// </summary>
        /// <param name="bytes">The value to encode.</param>
        /// <returns>Encoded string.</returns>
        public static string Encode(byte[] bytes)
        {
            strout.Clear();

            int b;

            for (int i = 0; i < bytes.Length; i += 3)
            {
                // #1 char
                b = (bytes[i] & 0xFC) >> 2;
                Append(b);

                b = (bytes[i] & 0x03) << 4;
                if (i + 1 < bytes.Length)
                {
                    // #2 char
                    b |= (bytes[i + 1] & 0xF0) >> 4;
                    Append(b);

                    b = (bytes[i + 1] & 0x0F) << 2;
                    if (i + 2 < bytes.Length)
                    {
                        // #3 char
                        b |= (bytes[i + 2] & 0xC0) >> 6;
                        Append(b);

                        // #4 char
                        b = bytes[i + 2] & 0x3F;
                        Append(b);
                    }
                    else
                    {
                        // #3 char, last char
                        Append(b);
                    }
                }
                else
                {
                    // #2 char, last char
                    Append(b);
                }
            }

            return strout.ToString();
        }

        /// <summary>
        /// Decode a previously encoded value.
        /// </summary>
        /// <param name="inChars">The encoded value</param>
        /// <returns>Original value</returns>
        public static byte[] Decode(char[] inChars)
        {
            // Map for special code followed by CODEFLAG '9] and its code index
            Codemap.Add('A', 61);
            Codemap.Add('B', 62);
            Codemap.Add('C', 63);

            var decodedList = new List<byte>();

            // 6-bit bytes
            var unit = new int[4];

            int inputLen = inChars.Length;

            // char counter
            int n = 0;

            // unit counter
            int m = 0;

            // regular char
            char ch1;

            // special char
            char ch2;

            byte b;

            while (n < inputLen)
            {
                ch1 = inChars[n];
                if (ch1 != CodeFlag)
                {
                    // regular code
                    unit[m] = Codes.IndexOf(ch1);
                    m++;
                    n++;
                }
                else
                {
                    n++;
                    if (n < inputLen)
                    {
                        ch2 = inChars[n];
                        if (ch2 != CodeFlag)
                        {
                            // special code index 61, 62, 63
                            unit[m] = Codemap[ch2];
                            m++;
                            n++;
                        }
                    }
                }

                // Add regular bytes with 3 bytes group composed from 4 units with 6 bits.
                if (m == 4)
                {
                    b = (byte)((unit[0] << 2) | (unit[1] >> 4));
                    decodedList.Add(b);
                    b = (byte)((unit[1] << 4) | (unit[2] >> 2));
                    decodedList.Add(b);
                    b = (byte)((unit[2] << 6 | unit[3]));
                    decodedList.Add(b);

                    // Reset unit counter
                    m = 0;
                }
            }

            // Add tail bytes group less than 4 units
            if (m != 0)
            {
                if (m == 1)
                {
                    b = (byte)((unit[0] << 2));
                    decodedList.Add(b);
                }
                else if (m == 2)
                {
                    b = (byte)((unit[0] << 2) | (unit[1] >> 4));
                    decodedList.Add(b);
                }
                else if (m == 3)
                {
                    b = (byte)((unit[0] << 2) | (unit[1] >> 4));
                    decodedList.Add(b);
                    b = (byte)((unit[1] << 4) | (unit[2] >> 2));
                    decodedList.Add(b);
                }
            }

            return decodedList.ToArray();
        }
    }
}
