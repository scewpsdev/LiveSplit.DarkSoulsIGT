using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LiveSplit.DarkSoulsIGT {
    public static class ASM {
        private static readonly Regex asmLineRx = new Regex(@"^[\w\d]+:\s+((?:[\w\d][\w\d] ?)+)");

        /// <summary>
        /// Convert string returned by https://defuse.ca/online-x86-assembler.htm to byte array
        /// Code by https://github.com/JKAnderson
        /// </summary>
        /// <param name="lines">out of https://defuse.ca/online-x86-assembler.htm </param>
        /// <returns>byte code</returns>
        private static byte[] loadDefuseOutput(string lines)
        {
            List<byte> bytes = new List<byte>();
            foreach (string line in Regex.Split(lines, "[\r\n]+"))
            {
                Match match = asmLineRx.Match(line);
                string hexes = match.Groups[1].Value;
                foreach (Match hex in Regex.Matches(hexes, @"\S+"))
                    bytes.Add(Byte.Parse(hex.Value, System.Globalization.NumberStyles.AllowHexSpecifier));
            }
            return bytes.ToArray();
        }

        public static byte[] CALLJAPAN = loadDefuseOutput(Properties.Resources.CallJapan);
    }
}
