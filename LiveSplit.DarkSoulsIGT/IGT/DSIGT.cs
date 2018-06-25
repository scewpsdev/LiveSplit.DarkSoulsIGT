using System;
using System.Diagnostics;

namespace LiveSplit.DarkSoulsIGT
{
    internal class DSIGT
    {
        private DSProcess gameProcess;
        private bool latch = true;
        private IntPtr IGTAddress;
        private Stopwatch stopwatch = new Stopwatch();
        private long lastUpdate;

        private int _IGT = 0;
        public int IGT
        {
            get
            {
                if (gameProcess.IsHooked)
                {
                    // 60 times per seconds
                    if (stopwatch.ElapsedMilliseconds - lastUpdate > DSIGTConfig.IGTRefreshRate)
                    {
                        IntPtr ptr = (IntPtr)DSMemory.RInt32(gameProcess.Process.Handle, IGTAddress);
                        int tmpIGT = DSMemory.RInt32(gameProcess.Process.Handle, IntPtr.Add(ptr, DSIGTConfig.Offset));

                        // If not in the main menu, update the timer normally
                        if (tmpIGT > 0)
                        {
                            _IGT = tmpIGT;
                            latch = false;
                        }

                        // If in the game menu and the timer hasn't be readjusted already...
                        if (tmpIGT == 0 && !latch)
                        {
                            // When you quitout, the game saves the IGT to the savefile but the timer
                            // actually keeps ticking for 18 more frames. So we remove that from the
                            // actual timer.
                            _IGT -= DSIGTConfig.TimerQuitoutDelay;
                            latch = true;
                        }

                        lastUpdate = stopwatch.ElapsedMilliseconds;
                    }
                }

                return _IGT;
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
            IGTAddress = gameProcess.Scan(DSIGTConfig.ArrayOfBytes, DSIGTConfig.ArrayOfByteOffset);
            lastUpdate = 0;
            stopwatch.Restart();
        }

        private void GameProcess_ProcessHasExited(object sender, EventArgs e)
        {
            IGTAddress = IntPtr.Zero;
            lastUpdate = 0;
            stopwatch.Stop();
        }

        public void Reset()
        {
            _IGT = 0;
            latch = true;
            lastUpdate = 0;
            stopwatch.Restart();
        }
    }
}
