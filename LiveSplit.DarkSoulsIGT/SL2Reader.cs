using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace LiveSplit.DarkSoulsIGT
{
    /*
    Encryption
        
    DS1:
        Files are not encrypted
    DSR:
        Each USERDATA file is individually AES-128-CBC encrypted. 
        Key:  01 23 45 67 89 AB CD EF FE DC BA 98 76 54 32 10 
        IV: First 16 bytes of each file
    */
    static class SL2Reader
    {
        private static readonly byte[] AES_KEY = { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF, 0xFE, 0xDC, 0xBA, 0x98, 0x76, 0x54, 0x32, 0x10 };

        /// <summary>
        /// Read the IGT from an SL2 file
        /// </summary>
        /// <param name="path">path to the SL2 file</param>
        /// <param name="slot">the slot to read the IGT from</param>
        /// <returns>IGT or -1 if sometimes failed</returns>
        public static int GetCurrentSlotIGT(string path, int slot)
        {
            int igt;

            if (path.Contains("DarkSouls"))
            {
                byte[] file = File.ReadAllBytes(path);
                int saveSlotSize = 0x60020;

                // Seems like GFWL files have a different slot size
                if (file.Length != 4326432)
                    saveSlotSize = 0x60190;

                int igtOffset = 0x2dc + (saveSlotSize * slot);
                igt = BitConverter.ToInt32(file, igtOffset);
            } else if (path.Contains("DARK SOULS REMASTERED"))
            {
                // Each USERDATA file is individually AES - 128 - CBC encrypted.
                byte[] file = File.ReadAllBytes(path);
                file = DecryptSL2(file);
                int saveSlotSize = 0x60030;
                int igtOffset = 0x2EC + (saveSlotSize * slot);
                igt = BitConverter.ToInt32(file, igtOffset);
            } else
            {
                igt = -1;
            }

            return igt;
        }

        /// <summary>
        /// Each USERDATA file is individually AES-128-CBC encrypted. 
        /// </summary>
        /// <param name="cipherBytes">encrypted bytes</param>
        /// <param name="key">key</param>
        /// <param name="iv">iv</param>
        /// <returns>decrypted bytes</returns>
        private static byte[] DecryptSL2(byte[] cipherBytes)
        {
            Aes encryptor = Aes.Create();
            encryptor.Mode = CipherMode.CBC;

            // Set key and IV
            byte[] aesKey = new byte[16];
            Array.Copy(AES_KEY, 0, aesKey, 0, 16);
            encryptor.Key = aesKey;
            encryptor.IV = cipherBytes.Take(16).ToArray();

            MemoryStream memoryStream = new MemoryStream();
            ICryptoTransform aesDecryptor = encryptor.CreateDecryptor();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, aesDecryptor, CryptoStreamMode.Write);
            byte[] plainBytes;

            try
            {
                cryptoStream.Write(cipherBytes, 0, cipherBytes.Length);
                cryptoStream.FlushFinalBlock();
                plainBytes = memoryStream.ToArray();
            }
            finally
            {
                memoryStream.Close();
                cryptoStream.Close();
            }

            return plainBytes;
        }
    }
}
