using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public bool Is64bit { get; set; }
        public GameVersion GameVersion { get => (!this.Is64bit) ? GameVersion.PrepareToDie : GameVersion.Remastered; }

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
            if (!IsHooked)
            {
                // After hooking, close all the remaining Processes immediately
                bool cleanup = false;

                // Looking for both ptde and remastered processes
                Process[] ptde = Process.GetProcessesByName(DSConfig.PrepareToDie);
                Process[] remastered = Process.GetProcessesByName(DSConfig.Remastered);
                Process[] candidates = ptde.Union(remastered).ToArray();

                foreach (Process process in candidates)
                {
                    bool close = false;

                    /**
                     * Checking if the process :
                     * - hasn't exited;
                     * - is responding;
                     * - has been running for at least 5 seconds.
                     */
                    close = cleanup || process.HasExited || !process.Responding || (DateTime.Now - process.StartTime).TotalMilliseconds < DSConfig.MinLifeSpan;

                    if (close)
                    {
                        // Closing processes we don't need
                        process.Close();
                    }
                    else
                    {
                        try
                        {
                            bool is64Bit = false;
                            if (DSMemory.IsWow64Process(process.Handle, out is64Bit))
                                is64Bit = !is64Bit;

                            this.Is64bit = is64Bit;
                            this._game = process;
                            this._game.EnableRaisingEvents = true;
                            this._game.Exited += Game_Exited;

                            memory = DSMemory.GetMemory(this._game);

                            // Game successfully hooked
                            ProcessHooked(this, EventArgs.Empty);
                        }
                        catch (Win32Exception)
                        {
                            // Probably IsWow64Process failed so we give up with that process
                            process.Close();
                        }
                    }
                }
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

        public IntPtr Scan(byte?[] aob, int offset)
        {
            /**
             * Use a different scan method depending on the game
             * Prepare To Die uses relative scanning while Remastered uses absolute ones
             */ 
            return (this.GameVersion == GameVersion.PrepareToDie)
                ? DSMemory.PrepareToDieScan(_game, memory, aob, offset)
                : DSMemory.RemasteredScan(_game, memory, aob, offset);
        }
    }
}
