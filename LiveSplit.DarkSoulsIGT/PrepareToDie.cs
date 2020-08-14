using System;
using PropertyHook;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace LiveSplit.DarkSoulsIGT
{
    class PrepareToDie : DarkSouls
    {
        /// <summary>
        /// Constants
        /// </summary>
        private const string CHR_DATA_AOB = "83 EC 14 A1 ? ? ? ? 8B 48 04 8B 40 08 53 55 56 57 89 4C 24 1C 89 44 24 20 3B C8";
        private const string CHAR_CLASS_BASE_AOB = "8B 0D ? ? ? ? 8B 7E 1C 8B 49 08 8B 46 20 81 C1 B8 01 00 00 57 51 32 DB";
        private const string CURRENT_SLOT_AOB = "8B 0D ? ? ? ? 80 B9 4F 0B 00 00 00 C6 44 24 28 00";
        private const string INVENTORY_RESET_AOB = "8B 4C 24 34 8B 44 24 2C 89 8A 38 01 00 00 8B 90 08 01 00 00 C1 E2 10 0B 90 00 01 00 00 8B C1 8B CD 89 14 AD ? ? ? ?";
        private const string SL2_INFORMATION_AOB = "00 00 00 00 66 89 0D ? ? ? ? C3 CC CC CC CC CC 83 3D";

        // lmao
        public PHPointer pSL2 { get; set; }

        /// <summary>
        /// Constructor
        /// Needs to RescanAOB() for pointers to update
        /// </summary>
        /// <param name="process"></param>
        public PrepareToDie(PHook process) : base(process)
        {
            // Set pointers
            pCharClassBase = Process.RegisterAbsoluteAOB(CHAR_CLASS_BASE_AOB, 2, 0);
            pInventoryReset = Process.RegisterAbsoluteAOB(INVENTORY_RESET_AOB, 36);
            pLoaded = Process.RegisterAbsoluteAOB(CHR_DATA_AOB, 4, 0, 0x4, 0x0);
            pCurrentSlot = Process.RegisterAbsoluteAOB(CURRENT_SLOT_AOB, 2, 0);
            pSL2 = Process.RegisterAbsoluteAOB(SL2_INFORMATION_AOB, 7);

            Process.RescanAOB();

            if (!Process.AOBScanSucceeded)
            {
                throw new Exception("At least one AOB scan failed.");
            }
        }

        /// <summary>
        /// Rollback value, used as a fallback
        /// </summary>
        public override int QuitoutRollback
        {
            get => 594;
        }

        /// <summary>
        /// Returns the raw IGT from Memory
        /// </summary>
        /// <returns></returns>
        public override int MemoryIGT
        {
            get => pCharClassBase.ReadInt32(0x68);
        }

        /// <summary>
        /// Current Save Slot index
        /// </summary>
        public override int CurrentSaveSlot
        {
            get => pCurrentSlot.ReadInt32(0xA70);
        }
        
        /// <summary>
        /// Current new game count
        /// </summary>
        public override int NgCount
        {
            get => pCharClassBase.ReadInt32(0x003C);
        }

        /// <summary>
        /// Read a Unicode string from the memory
        /// </summary>
        /// <param name="pointer"></param>
        /// <returns></returns>
        private string ReadUnicode(PHPointer pointer)
        {
            var buff = new List<byte>(128);

            for (int i = 0; ; i += 2)
            {
                byte[] b = pointer.ReadBytes(i, 2);
                if (b[0] == 0 && b[1] == 0)
                    break;
                buff.AddRange(b);
            }

            return Encoding.Unicode.GetString(buff.ToArray());
        }

        /// <summary>
        /// Returns the savefile's location
        /// </summary>
        /// <returns></returns>
        public override string GetSaveFileLocation()
        {
            var MyDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var path = Path.Combine(MyDocuments, "NBGI\\DarkSouls");
            var variable = pSL2.ReadInt32(0x10);

            if (variable != 0)
            {
                string gfwl_id = ReadUnicode(Process.CreateChildPointer(pSL2, 0x0));
                path = Path.Combine(path, gfwl_id);
            }

            return Path.Combine(path, "DRAKS0005.sl2");
        }
    }
}
