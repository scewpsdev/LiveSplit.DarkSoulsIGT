using System;
using PropertyHook;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using System.Linq;

namespace LiveSplit.DarkSoulsIGT
{
    class Remastered : DarkSouls
    {
        /// <summary>
        /// Constants
        /// </summary>
        private byte[] AES_KEY = { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF, 0xFE, 0xDC, 0xBA, 0x98, 0x76, 0x54, 0x32, 0x10 };
        private const string CHR_DATA_AOB = "48 8B 0D ? ? ? ? E8 ? ? ? ? 48 8B 4E 68 48 8B 05 ? ? ? ? 48 89 48 60";
        private const string CHR_FOLLOW_CAM_AOB = "48 8B 0D ? ? ? ? E8 ? ? ? ? 48 8B 4E 68 48 8B 05 ? ? ? ? 48 89 48 60";
        private const string INVENTORY_RESET_AOB = "4C 8B 65 EF 4C 8B 6D 0F 49 63 C6 48 8D 0D ? ? ? ? 8B 14 81 83 FA FF";
        private const string BASE_PTR_AOB = "48 8B 05 ? ? ? ? 48 85 C0 ? ? F3 0F 58 80 AC 00 00 00";
        private const string CHR_CLASS_WARP_AOB = "48 8B 05 ? ? ? ? 66 0F 7F 80 A0 0B 00 00 0F 28 02 66 0F 7F 80 B0 0B 00 00 C6 80";

        private PHPointer pBasePtr { get; set; }

        /// <summary>
        /// Constructor
        /// Needs to RescanAOB() for pointers to update
        /// </summary>
        /// <param name="process"></param>
        public Remastered(PHook process) : base(process)
        {
            // Set pointers
            pBasePtr = Process.RegisterRelativeAOB(BASE_PTR_AOB, 3, 7);
            pLoaded = Process.RegisterRelativeAOB(CHR_FOLLOW_CAM_AOB, 3, 7, 0, 0x60, 0x60);
            pInventoryReset = Process.RegisterRelativeAOB(INVENTORY_RESET_AOB, 14, 0);
            pCurrentSlot = Process.RegisterRelativeAOB(CHR_CLASS_WARP_AOB, 3, 0, 7);

            pIGT = Process.CreateChildPointer(pBasePtr, 0);
            pNgCount = Process.CreateChildPointer(pBasePtr, 0);

            Process.RescanAOB();
        }

        /// <summary>
        /// Returns the raw IGT from Memory
        /// </summary>
        /// <returns></returns>
        public override int MemoryIGT
        {
            get => pIGT.ReadInt32(0xA4);
        }

        /// <summary>
        /// Current Save Slot index
        /// </summary>
        public override int CurrentSaveSlot
        {
            get => pCurrentSlot.ReadInt32(0xAA0);
        }

        /// <summary>
        /// Current new game count
        /// </summary>
        public override int NgCount
        {
            get => pNgCount.ReadInt32(0x78);
        }

        /// <summary>
        /// 
        /// </summary>
        public override string SaveFilePath
        {
            get
            {
                var MyDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                return Path.Combine(MyDocuments, "NBGI\\DARK SOULS REMASTERED\\84341421\\DRAKS0005.sl2").ToString();
            }
        }

        /// <summary>
        /// Returns the IGT of a specific slot in the game's savefile
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        public override int GetCurrentSlotIGT(int slot = 0)
        {
            int igt = 0;

            if (slot >= 0 && slot <= 10)
            {
                try
                {
                    byte[] file = File.ReadAllBytes(SaveFilePath);
                    byte[] decrypted = DecryptSL2(file, AES_KEY, AES_KEY);
                    int saveSlotSize = 0x60030;
                    int igtOffset = 0x2EC + saveSlotSize * CurrentSaveSlot;
                    igt = BitConverter.ToInt32(decrypted, igtOffset);
                }
                catch
                {
                    igt = 0;
                }
            }

            return igt;
        }

        /// <summary>
        /// Decrypts SL2 file for DSR cause this meme is using AES
        /// </summary>
        /// <param name="cipherBytes"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        private byte[] DecryptSL2(byte[] cipherBytes, byte[] key, byte[] iv)
        {
            Aes encryptor = Aes.Create();
            encryptor.Mode = CipherMode.CBC;

            // Set key and IV
            byte[] aesKey = new byte[16];
            Array.Copy(key, 0, aesKey, 0, 16);
            encryptor.Key = aesKey;
            encryptor.IV = iv;

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
