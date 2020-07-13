using System;
using PropertyHook;
using System.IO;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace LiveSplit.DarkSoulsIGT
{
    class Remastered : DarkSouls
    {
        /// <summary>
        /// Constants
        /// </summary>
        private byte[] AES_KEY = { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF, 0xFE, 0xDC, 0xBA, 0x98, 0x76, 0x54, 0x32, 0x10 };
        private const string CHR_FOLLOW_CAM_AOB = "48 8B 0D ? ? ? ? E8 ? ? ? ? 48 8B 4E 68 48 8B 05 ? ? ? ? 48 89 48 60";
        private const string INVENTORY_RESET_AOB = "4C 8B 65 EF 4C 8B 6D 0F 49 63 C6 48 8D 0D ? ? ? ? 8B 14 81 83 FA FF";
        private const string CHR_CLASS_BASE_AOB = "48 8B 05 ? ? ? ? 48 85 C0 ? ? F3 0F 58 80 AC 00 00 00";
        private const string CHR_CLASS_WARP_AOB = "48 8B 05 ? ? ? ? 66 0F 7F 80 A0 0B 00 00 0F 28 02 66 0F 7F 80 B0 0B 00 00 C6 80";

        /// <summary>
        /// Constructor
        /// Needs to RescanAOB() for pointers to update
        /// </summary>
        /// <param name="process"></param>
        public Remastered(PHook process) : base(process)
        {
            // Set pointers
            pCharClassBase = Process.RegisterRelativeAOB(CHR_CLASS_BASE_AOB, 3, 7, 0);
            pLoaded = Process.RegisterRelativeAOB(CHR_FOLLOW_CAM_AOB, 3, 7, 0, 0x60, 0x60);
            pInventoryReset = Process.RegisterRelativeAOB(INVENTORY_RESET_AOB, 14, 0x12);
            pCurrentSlot = Process.RegisterRelativeAOB(CHR_CLASS_WARP_AOB, 3, 0, 7);

            Process.RescanAOB();
        }

        /// <summary>
        /// SteamID3 used for savefile location
        /// </summary>
        public int SteamID3
        {
            get {
                int id = 0;

                if (Process.Hooked)
                {
                    foreach (ProcessModule item in Process.Process.Modules)
                    {
                        if (item.ModuleName == "steam_api64.dll")
                        {
                            id = Process.CreateBasePointer(Process.Handle, 0).ReadInt32((int)item.BaseAddress + 0x38E58);
                            break;
                        }
                    }
                }

                return id;
            }
        }

        /// <summary>
        /// Rollback value, used as a fallback
        /// </summary>
        public override int QuitoutRollback
        {
            get => 512;
        }

        /// <summary>
        /// Returns the raw IGT from Memory
        /// </summary>
        /// <returns></returns>
        public override int MemoryIGT
        {
            get => pCharClassBase.ReadInt32(0xA4);
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
            get => pCharClassBase.ReadInt32(0x78);
        }

        /// <summary>
        /// Returns the savefile's location
        /// </summary>
        /// <returns></returns>
        public override string GetSaveFileLocation()
        {
            // @TODO find a pointer to the player's region since only Japan gets
            // a different savefile folder
            var MyDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var japan = Path.Combine(MyDocuments, "FromSoftware\\DARK SOULS REMASTERED");
            var path = Path.Combine(MyDocuments, "NBGI\\DARK SOULS REMASTERED");

            if (Directory.Exists(japan))
            {
                path = japan;
            }

            return Path.Combine(path, $"{SteamID3}\\DRAKS0005.sl2");
        }
    }
}
