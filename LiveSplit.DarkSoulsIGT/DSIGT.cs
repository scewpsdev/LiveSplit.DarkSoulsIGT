using System;
using System.Diagnostics;
using System.Linq;

namespace LiveSplit.DarkSoulsIGT
{
    internal class DSIGT
    {
        private Process _game;
        private IntPtr _IGTPointer;

        private bool _latch = true;
        private int _IGT = 0;
        public int IGT
        {
            get
            {
                if (_game != null && !_game.HasExited && _IGTPointer != IntPtr.Zero)
                {
                    IntPtr ptr = (IntPtr)DSIGTMemory.RInt32(_game.Handle, _IGTPointer);
                    int tmpIGT = DSIGTMemory.RInt32(_game.Handle, IntPtr.Add(ptr, DSIGTConfig.Offsets));

                    // If not in the main menu, update the timer normally
                    if (tmpIGT > 0)
                    {
                        _IGT = tmpIGT;
                        _latch = false;
                    }

                    // If in the game menu and the timer hasn't be readjusted already...
                    if (tmpIGT == 0 && !_latch)
                    {
                        // When you quitout, the game saves the IGT to the savefile but the timer
                        // actually keeps ticking for 18 more frames. So we remove that from the
                        // actual timer.
                        _IGT -= DSIGTConfig.TimerQuitoutDelay; // 594ms or 33ms * 18
                        _latch = true;
                    }
                    
                    return _IGT;
                }
                else
                {
                    // Tries to hook the game for the next call
                    Hook();
                    return _IGT;
                }
            }
        }

        public DSIGT()
        {
            Hook();
        }

        public void Reset()
        {
            _IGT = 0;
            _latch = true;
        }

        private Process GetProcess()
        {
            Process tmp = null;
            Process[] candidates = Process.GetProcessesByName(DSIGTConfig.Module);
            if (candidates.Length > 0 && !candidates.First().HasExited)
            {
                tmp = candidates.First();
                tmp.EnableRaisingEvents = true;
                tmp.Exited += Game_Exited;
            }

            return tmp;
        }

        private void Game_Exited(object sender, EventArgs e)
        {
            // Game closed or crashed
            Unhook();
        }

        private void Hook()
        {
            _game = GetProcess();
            if (_game != null)
            {
                // Support for both Release and Debug
                uint gameType = DSIGTMemory.RUInt32(_game.Handle, DSIGTConfig.VersionCheck);
                if (gameType == DSIGTConfig.VersionRelease)
                {
                    _IGTPointer = DSIGTConfig.BasePointerRelease;
                }
                else if (gameType == DSIGTConfig.VersionDebug)
                {
                    _IGTPointer = DSIGTConfig.BasePointerDebug;
                }
                else
                {
                    // Game found but invalid game version
                    _game = null;
                    _IGTPointer = IntPtr.Zero;
                }
            }
        }

        public void Unhook()
        {
            _game = null;
            _IGTPointer = IntPtr.Zero;
        }
    }
}
