using System;
using PropertyHook;
using System.IO;
using System.Diagnostics;

namespace LiveSplit.DarkSoulsIGT {
    class Remastered : DarkSouls {
        /// <summary>
        /// Constants
        /// </summary>
        private const string CHR_FOLLOW_CAM_AOB = "48 8B 0D ? ? ? ? E8 ? ? ? ? 48 8B 4E 68 48 8B 05 ? ? ? ? 48 89 48 60";
        private const string INVENTORY_RESET_AOB = "48 8D 15 ? ? ? ? C1 E1 10 49 8B C6 41 0B 8F 14 02 00 00 44 8B C6 42 89 0C B2 41 8B D6 49 8B CF";
        private const string CHR_CLASS_BASE_AOB = "48 8B 05 ? ? ? ? 48 85 C0 ? ? F3 0F 58 80 AC 00 00 00";
        private const string CHR_CLASS_WARP_AOB = "48 8B 05 ? ? ? ? 66 0F 7F 80 A0 0B 00 00 0F 28 02 66 0F 7F 80 B0 0B 00 00 C6 80";

        /// <summary>
        /// Private variables
        /// </summary>
        private int? steamId3 = null;
        private bool? isJapanese = false;

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
            pInventoryReset = Process.RegisterRelativeAOB(INVENTORY_RESET_AOB, 3, 7);
            pCurrentSlot = Process.RegisterRelativeAOB(CHR_CLASS_WARP_AOB, 3, 0, 7);

            Process.RescanAOB();

            if (!Process.AOBScanSucceeded)
            {
                throw new Exception("At least one AOB scan failed.");
            }

            // Only compute those values once
            steamId3 = SteamID3();
            isJapanese = IsJapanese();
        }

        /// <summary>
        /// Returns true if the game currently is in Japanese
        /// </summary>
        /// <returns>bool</returns>
        private bool IsJapanese()
        {
            /**
             * Calls DarkSoulsRemastered.exe+C8820 and then writes the value of eax
             * to a given address. If that value is 0, the game is in Japanese.
             * That function uses the steam64 api underneath so we have no other
             * choice but calling that function manually
             */
            IntPtr callPtr = IntPtr.Add(Process.Process.MainModule.BaseAddress, 0xC8820);
            IntPtr resultPtr = Process.Allocate(0x4, Kernel32.PAGE_READWRITE);

            // build asm and replace the function pointers
            byte[] asm = (byte[])ASM.CALLJAPAN.Clone();
            byte[] callBytes = BitConverter.GetBytes((ulong)callPtr);
            Array.Copy(callBytes, 0, asm, 0x6, 8);
            byte[] resultBytes = BitConverter.GetBytes((ulong)resultPtr);
            Array.Copy(resultBytes, 0, asm, 0x13, 8);

            Process.Execute(asm);
            bool isJapanese = Process.CreateBasePointer(resultPtr).ReadInt32(0) == 0;

            Process.Free(resultPtr);
            return isJapanese;
        }

        /// <summary>
        /// SteamID3 used for savefile location
        /// </summary>
        public int SteamID3()
        {
            string name = "steam_api64.dll";
            ProcessModule module = null;

            foreach (ProcessModule item in Process.Process.Modules)
            {
                if (item.ModuleName == name)
                {
                    module = item;
                    break;
                }
            }

            if (module == null)
            {
                throw new DllNotFoundException($"${name} not found");
            }

            return Process.CreateBasePointer(Process.Handle, 0).ReadInt32((int)module.BaseAddress + 0x38E58); ;
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
            // values may be null if called before hook, in which
            // case we can't guess the savefile location
            if (isJapanese == null || steamId3 == null)
            {
                return null;
            }

            var MyDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var japan = Path.Combine(MyDocuments, "FromSoftware\\DARK SOULS REMASTERED");
            var path = Path.Combine(MyDocuments, "NBGI\\DARK SOULS REMASTERED");

            if (isJapanese == true)
            {
                path = japan;
            }

            if (steamId3 != null)
            {
                path = Path.Combine(path, $"{steamId3}");
            }

            return Path.Combine(path, "DRAKS0005.sl2");
        }
    }
}
