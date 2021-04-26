using System;
using PropertyHook;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using LiveSplit.ComponentUtil;

namespace LiveSplit.DarkSoulsIGT
{
    public class PTDE : DarkSouls {
        public override GameVersion Version => GameVersion.PrepareToDie;
        protected override int rollbackMs => 594;

        protected override SigScanTarget inGameTime
        {
            get
            {
                SigScanTarget target = new SigScanTarget(2, "8B 0D ?? ?? ?? ?? 8B 7E 1C 8B 49 08 8B 46 20 81 C1 B8 01 00 00 57 51 32 DB");
                target.OnFound += (Process process, SignatureScanner scanner, IntPtr ptr) =>
                {
                    IntPtr pointer = IntPtr.Zero;
                    DeepPointer deepPointer = new DeepPointer(ptr, 0, 0x68);
                    deepPointer.DerefOffsets(process, out pointer);
                    return pointer;
                };

                return target;
            }
        }

        protected override SigScanTarget currentSaveSlot
        {
            get
            {
                SigScanTarget target = new SigScanTarget(2, "8B 0D ?? ?? ?? ?? 80 B9 4F 0B 00 00 00 C6 44 24 28 00");
                target.OnFound += (Process process, SignatureScanner scanner, IntPtr ptr) =>
                {
                    IntPtr pointer = IntPtr.Zero;
                    DeepPointer deepPointer = new DeepPointer(ptr, 0, 0xA70);

                    if (!deepPointer.DerefOffsets(process, out pointer))
                    {
                        throw new Exception("Failed to scan currentSaveSlot");
                    }

                    return pointer;
                };

                return target;
            }
        }

        protected override SigScanTarget inventoryIndex
        {
            get
            {
                SigScanTarget target = new SigScanTarget(36, "8B 4C 24 34 8B 44 24 2C 89 8A 38 01 00 00 8B 90 08 01 00 00 C1 E2 10 0B 90 00 01 00 00 8B C1 8B CD 89 14 AD ?? ?? ?? ??");
                target.OnFound += (Process process, SignatureScanner scanner, IntPtr ptr) =>
                {
                    IntPtr pointer = IntPtr.Zero;
                    DeepPointer deepPointer = new DeepPointer(ptr, 0);

                    if (!deepPointer.DerefOffsets(process, out pointer))
                    {
                        throw new Exception("Failed to scan inventoryIndex");
                    }

                    return pointer;
                };

                return target;
            }
        }

        protected override SigScanTarget flags
        {
            get
            {
                SigScanTarget target = new SigScanTarget(8, "56 8B F1 8B 46 1C 50 A1 ?? ?? ?? ?? 32 C9");
                target.OnFound += (Process process, SignatureScanner scanner, IntPtr ptr) =>
                {
                    IntPtr pointer = IntPtr.Zero;
                    DeepPointer deepPointer = new DeepPointer(ptr, 0, 0, 0);

                    if (!deepPointer.DerefOffsets(process, out pointer))
                    {
                        throw new Exception("Failed to scan flags");
                    }

                    return pointer;
                };

                return target;
            }
        }


        public PTDE(Process proc) : base(proc)
        {

        }

        public override string GetSaveFileLocation()
        {
            var MyDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var path = Path.Combine(MyDocuments, "NBGI\\DarkSouls");

            //var variable = pSL2.ReadInt32(0x10);
            //if (variable != 0)
            //{
            //    string gfwl_id = ReadUnicode(Process.CreateChildPointer(pSL2, 0x0));
            //    path = Path.Combine(path, gfwl_id);
            //}

            return Path.Combine(path, "DRAKS0005.sl2");
        }
    }
}
