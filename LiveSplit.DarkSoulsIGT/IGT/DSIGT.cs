using System;

namespace LiveSplit.DarkSoulsIGT
{
    internal class DSIGT
    {
        private DSProcess gameProcess;
        private bool latch = true;
        private IntPtr IGTAddress;

        private int LocalIGT = 0;
        public int IGT
        {
            get
            {
                if (gameProcess.IsHooked)
                {
                    int offset = DSIGTConfig.Offset[this.gameProcess.GameVersion];

                    IntPtr ptr = (IntPtr)DSMemory.RInt32(gameProcess.Process.Handle, IGTAddress);
                    int tmpIGT = DSMemory.RInt32(gameProcess.Process.Handle, IntPtr.Add(ptr, offset));

                    // If not in the main menu, update the timer normally
                    if (tmpIGT > 0)
                    {
                        LocalIGT = tmpIGT;
                        latch = false;
                    }

                    // If in the game menu and the timer hasn't be readjusted already...
                    if (tmpIGT == 0 && !latch)
                    {
                        // When you quitout, the game saves the IGT to the savefile but the timer
                        // actually keeps ticking for a few frames. So we remove that from the
                        // actual timer.
                        LocalIGT -= DSIGTConfig.TimerQuitoutDelay[this.gameProcess.GameVersion];
                        latch = true;
                    }
                }

                return LocalIGT;
            }
        }

        public DSIGT(DSProcess gameProcess)
        {
            this.gameProcess = gameProcess;
            this.gameProcess.ProcessHooked += GameProcess_ProcessHooked;
            this.gameProcess.ProcessHasExited += GameProcess_ProcessHasExited;
        }

        private void GameProcess_ProcessHooked(object sender, EventArgs e)
        {
            byte?[] aob = DSIGTConfig.ArrayOfBytes[this.gameProcess.GameVersion];
            int offset = DSIGTConfig.ArrayOfByteOffset[this.gameProcess.GameVersion];

            IGTAddress = gameProcess.Scan(aob, offset);
        }

        private void GameProcess_ProcessHasExited(object sender, EventArgs e)
        {
            IGTAddress = IntPtr.Zero;
        }

        public void Reset()
        {
            LocalIGT = 0;
            latch = true;
        }
    }
}
