using System;

namespace LiveSplit.DarkSoulsIGT
{
    internal class DSInventoryReset
    {
        private DSProcess gameProcess;
        private IntPtr IRaddress;

        public DSInventoryReset(DSProcess gameProcess)
        {
            this.gameProcess = gameProcess;
            this.gameProcess.ProcessHooked += GameProcess_ProcessHooked;
            this.gameProcess.ProcessHasExited += GameProcess_ProcessHasExited;
        }

        private void GameProcess_ProcessHooked(object sender, EventArgs e)
        {
            byte?[] aob = DSInventoryIndexConfig.ArrayOfBytes[this.gameProcess.GameVersion];
            int offset = DSInventoryIndexConfig.ArrayOfByteOffset[this.gameProcess.GameVersion];

            IRaddress = gameProcess.Scan(aob, offset);
        }

        private void GameProcess_ProcessHasExited(object sender, EventArgs e)
        {
            IRaddress = IntPtr.Zero;
        }

        public void ResetInventory()
        {
            if (gameProcess.IsHooked)
            {
                // Foreach equipement slot...
                foreach (IntPtr equipementSlot in DSInventoryIndexConfig.GetInventoryAddresses(IRaddress))
                {
                    // Gets the address to the value of the index
                    DSMemory.WUInt32(gameProcess.Process.Handle, equipementSlot, UInt32.MaxValue); // Sets to the default value (4,294,967,295)
                }
            }
        }
    }
}
