using System;
using System.Collections.Generic;
using System.Diagnostics;
using LiveSplit.ComponentUtil;

namespace LiveSplit.DarkSoulsIGT {
    public abstract class DarkSouls {
        private SignatureScanner scanner;
        private Process proc;

        #region Watchers
        public MemoryWatcher<int> inGameTimeWatcher;
        public MemoryWatcher<int> currentSaveSlotWatcher;
        public Dictionary<IntPtr, MemoryWatcher<int>> flagsWatchers = new Dictionary<IntPtr, MemoryWatcher<int>>();

        public List<MemoryWatcher> AllWatchers => new List<MemoryWatcher>(flagsWatchers.Values)
        {
            inGameTimeWatcher,
            currentSaveSlotWatcher,
        };
        #endregion

        public event OnInGameTimeChangedEventHandler OnInGameTimeChanged;
        public event OnBossDiedEventHandler OnBossDied;
        public event OnQuitoutEventHandler OnQuitout;

        public DarkSouls(Process proc)
        {
            this.proc = proc;
            scanner = new SignatureScanner(proc, proc.MainModule.BaseAddress, proc.MainModule.ModuleMemorySize);

            inGameTimeWatcher = new MemoryWatcher<int>(scanner.Scan(inGameTime));
            currentSaveSlotWatcher = new MemoryWatcher<int>(scanner.Scan(currentSaveSlot));

            // Watch for boss flags
            IntPtr flagsBaseAddress = scanner.Scan(flags);
            foreach (var boss in Flags.Bosses)
            {
                int offset = Flags.GetEventFlagOffset(boss.FlagID, out uint mask);
                IntPtr pointer = IntPtr.Add(flagsBaseAddress, offset);

                if (!flagsWatchers.ContainsKey(pointer))
                {
                    flagsWatchers.Add(pointer, new MemoryWatcher<int>(pointer));
                }

                flagsWatchers[pointer].OnChanged += (old, current) =>
                {
                    bool wasDead = (old & mask) == mask;
                    bool isDead = (current & mask) == mask;

                    if (!wasDead && isDead)
                    {
                        OnBossDied?.Invoke(boss);
                    }
                };
            }

            inGameTimeWatcher.OnChanged += InGameTimeWatcher_OnChanged;
            proc.Exited += Proc_Exited;
        }

        /// <summary>
        /// Stop listening to watchers on process exit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Proc_Exited(object sender, EventArgs e)
        {
            if (inGameTimeWatcher != null)
                inGameTimeWatcher.OnChanged -= InGameTimeWatcher_OnChanged;
        }

        /// <summary>
        /// Process the IGT based on the Memory IGT and the Savefile's IGT
        /// </summary>
        /// <param name="old">Memory's old IGT</param>
        /// <param name="current">Memory's new IGT</param>
        private void InGameTimeWatcher_OnChanged(int old, int current)
        {
            int _old = old;
            int _current = current;

            if (_current == 0 && _old > 0)
            {
                OnQuitout?.Invoke();

                int slot = currentSaveSlotWatcher.Current;
                string path = GetSaveFileLocation();

                if (SL2Reader.GetCurrentSlotIGT(path, slot, Version, out int savefileIGT))
                {
                    _current = savefileIGT;
                }
                else
                {
                    // Failed to read IGT from savefile, fallback to rollback method
                    _current = _old - rollbackMs;
                }
            }

            OnInGameTimeChanged?.Invoke(_old, _current);
        }

        /// <summary>
        /// Reset the inventory indexes if possible
        /// </summary>
        public void ResetIndexes()
        {
            if (!proc.HasExited)
            {
                IntPtr pointer = scanner.Scan(inventoryIndex);

                for (int i = 0; i < 20; i++) // the player's inventory has 20 slots
                {
                    if (!ExtensionMethods.WriteValue(proc, IntPtr.Add(pointer, 0x4 * i), uint.MaxValue))
                    {
                        throw new Exception($"Inventory index reset failed (used 0x{pointer.ToString("X8")} as a base address)");
                    }
                }
            }
        }

        /// <summary>
        /// Main update loop
        /// </summary>
        public void Update()
        {
            if (proc.HasExited) return;
            AllWatchers.ForEach(w => w?.Update(proc));
        }

        public void Reset()
        {
            AllWatchers.ForEach(w => w?.Reset());
        }

        public abstract GameVersion Version { get; }
        public abstract string GetSaveFileLocation();
        protected abstract int rollbackMs { get; }
        protected abstract SigScanTarget inGameTime { get; }
        protected abstract SigScanTarget currentSaveSlot { get; }
        protected abstract SigScanTarget inventoryIndex { get; }
        protected abstract SigScanTarget flags { get; }

        public delegate void OnInGameTimeChangedEventHandler(int old, int current);
        public delegate void OnBossDiedEventHandler(BossFlag boss);
        public delegate void OnQuitoutEventHandler();
    }
}
