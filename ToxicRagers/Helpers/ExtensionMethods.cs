﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ToxicRagers.Helpers
{
    public static class ExtensionMethods
    {
        // Rather specific versions of Read and WriteString here.  Possibly move inside cFBX.cs
        public static string ReadPropertyString(this BinaryReader br, int length)
        {
            var c = br.ReadChars(length);
            int l = length;

            for (int i = 0; i < length - 1; i++)
            {
                if (c[i+0] == 0 && c[i+1] == 1)
                {
                    c[i + 0] = ':';
                    c[i + 1] = ':';
                    break;
                }
            }

            return new string(c, 0, l);
        }

        public static void WritePropertyString(this BinaryWriter bw, string s)
        {
            var c = s.ToCharArray();
            int length = c.Length;

            for (int i = 0; i < length - 1; i++)
            {
                if (c[i + 0] == ':' && c[i + 1] == ':')
                {
                    c[i + 0] = (char)0;
                    c[i + 1] = (char)1;
                    break;
                }
            }

            bw.Write(c);
        }

        public static string ReadString(this BinaryReader br, int length)
        {
            var c = br.ReadChars(length);
            int l = length;

            for (int i = 0; i < length; i++)
            {
                if (c[i] == 0)
                {
                    l = i;
                    break;
                }
            }

            return new string(c, 0, l);
        }

        public static string[] ReadStrings(this BinaryReader br, int count)
        {
            var r = new string[count];
            int i = 0;

            while (i < count)
            {
                var c = br.ReadChar();
                if (c == 0)
                {
                    i++;
                }
                else
                {
                    r[i] += c;
                }
            }

            return r;
        }

        public static void WriteString(this BinaryWriter bw, string s)
        {
            bw.Write(s.ToCharArray());
        }

        public static void Add(this Dictionary<string, string> d, string[] kvp)
        {
            d[kvp[0]] = string.Join(" ", kvp, 1, kvp.Length - 1);
        }

        public static string ToName(this byte[] b)
        {
            string s = "";

            for (int i = 0; i < b.Length; i++)
            {
                if (b[i] == 0) { break; }
                s += (char)b[i];
            }

            return s;
        }

        public static string Replace(this string originalString, string oldValue, string newValue, StringComparison comparisonType)
        {
            int startIndex = 0;
            while (true)
            {
                startIndex = originalString.IndexOf(oldValue, startIndex, comparisonType);
                if (startIndex == -1)
                    break;

                originalString = originalString.Substring(0, startIndex) + newValue + originalString.Substring(startIndex + oldValue.Length);

                startIndex += newValue.Length;
            }

            return originalString;
        }

        public static string ToFormattedString(this byte[] ba)
        {
            int count = ba.Length;
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            for (int i = 0; i < count; i++) { hex.AppendFormat("0x{0:x2}{1}", ba[i], (i + 1 < count ? ", " : "")); }
            return hex.ToString();
        }

        public static string ToFormattedString<T>(this T a)
            where T : IEnumerable
        {
            return string.Join(",", a);
        }
    }
}
