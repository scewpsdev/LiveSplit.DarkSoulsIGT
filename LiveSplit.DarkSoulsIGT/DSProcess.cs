using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LiveSplit.DarkSoulsIGT
{
    internal class DSProcess
    {
        public event EventHandler ProcessHooked;
        public event EventHandler ProcessHasExited;

        private Process _game = null;
        private Dictionary<IntPtr, byte[]> memory = new Dictionary<IntPtr, byte[]>();

        public bool IsHooked { get => _game != null && !_game.HasExited; }
        public Process Process { get => _game; }
        public GameVersion GameVersion { get; set; }

        public void Update()
        {
            if (!IsHooked)
            {
                Hook();
            }
        }

        public void Dispose()
        {
            Unhook();
        }

        private void Hook()
        {
            Process[] candidates = Process.GetProcessesByName(DSConfig.Module);
            if (candidates.Length > 0 && !candidates.First().HasExited)
            {
                _game = candidates.First();
                _game.EnableRaisingEvents = true;
                _game.Exited += Game_Exited;

                memory = DSMemory.GetMemory(_game);
                ProcessHooked(this, EventArgs.Empty);
            }
            else
            {
                Unhook();
            }
        }

        private void Game_Exited(object sender, EventArgs e)
        {
            Unhook();
        }

        private void Unhook()
        {
            _game = null;
            memory = new Dictionary<IntPtr, byte[]>();

            ProcessHasExited(this, EventArgs.Empty);
        }

        public IntPtr Scan(byte?[] aob)
        {
            return DSMemory.Scan(_game, memory, aob);
        }

        public IntPtr Scan(byte?[] aob, int offset)
        {
            return DSMemory.Scan(_game, memory, aob, offset);
        }
    }
}
