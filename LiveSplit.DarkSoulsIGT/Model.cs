using System;
using PropertyHook;
using System.Diagnostics;

namespace LiveSplit.DarkSoulsIGT
{
    internal enum GameVersion
    {
        PrepareToDie,
        Remastered,
    }

    class Model : PHook
    {
        /// <summary>
        /// Constants
        /// </summary>
        private const string PTDE_NAME = "DARKSOULS";
        private const string REMASTERED_NAME = "DARK SOULS™: REMASTERED";
        private const int UNHOOKED_INTERVAL = 1000;
        private const int HOOKED_INTERVAL = 33;
        private const int MIN_LIFE_SPAN = 5000;

        /// <summary>
        /// Local private variables
        /// </summary>
        private int localIGT;
        private bool quitoutLatch;
        private bool creditsLatch;
        private int previousNgCount;
        private bool ngChanged;
        private bool creditsRolling;

        /// <summary>
        /// Process Selector
        /// </summary>
        private static Func<Process, bool> processSelector = (p) =>
        {
            return (p.MainWindowTitle == REMASTERED_NAME) || (p.ProcessName == PTDE_NAME);
        };

        /// <summary>
        /// Abstract DarkSouls class
        /// Could be null
        /// </summary>
        private DarkSouls DarkSouls { get; set; }

        /// <summary>
        /// Dynanux RefreshInterval
        /// </summary>
        new public int RefreshInterval
        {
            get
            {
                return (Hooked) ? HOOKED_INTERVAL : UNHOOKED_INTERVAL;
            }
        }

        /// <summary>
        /// If the model is ready to be used or not
        /// </summary>
        public bool Ready
        {
            get => Hooked && DarkSouls != null;
        }

        /// <summary>
        /// Game version. Only remastered is 64 bits
        /// </summary>
        public GameVersion Version
        {
            get
            {
                return (Is64Bit) ? GameVersion.Remastered : GameVersion.PrepareToDie;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Model() : base(0, MIN_LIFE_SPAN, processSelector) // RefreshInterval at 0 because manually refreshed from the outside
        {
            OnHooked += DarkSoulsProcess_OnHooked;
            OnUnhooked += DarkSoulsProcess_OnUnhooked;
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            if (Process != null)
            {
                Process.Dispose();
            }
        }

        /// <summary>
        /// Reset some variables when needed
        /// </summary>
        public void Reset()
        {
            localIGT = 0;
            quitoutLatch = true;
            creditsLatch = false;
            ngChanged = false;
            creditsRolling = false;

            if (Ready)
            {
                previousNgCount = DarkSouls.NgCount;
            }
        }

        /// <summary>
        /// Resets the inventory indexes if possible
        /// </summary>
        public void ResetIndexes()
        {
            if (Ready)
            {
                DarkSouls.ResetInventory();
            }
        }

        /// <summary>
        /// Returns the IGT
        /// Reads the savefile's IGT on quitouts and credits
        /// Reads the memory IGT otherwise
        /// </summary>
        public int GetInGameTime()
        {
            if (Ready)
            {
                int _tmpIGT = DarkSouls.MemoryIGT;
                int _tmpNgCount = DarkSouls.NgCount;
                bool _isLoaded = DarkSouls.Loaded;

                // If IGT is running
                if (_tmpIGT > 0)
                {
                    // then we're not on a quitout
                    quitoutLatch = false;

                    // Check if NG increased since last update (in-game cutscene is playing)
                    if (_tmpNgCount > previousNgCount)
                    {
                        // Final in-game cutscene is playing, just keep using memory IGT
                        if (_isLoaded && !creditsRolling)
                        {
                            localIGT = _tmpIGT;
                            return localIGT;
                        }

                        // if the player isn't loaded, then credits are playing (video cutscene is playing)
                        if (!_isLoaded && !creditsRolling)
                        {                          
                            int fileIGT = DarkSouls.GetCurrentSlotIGT(DarkSouls.CurrentSaveSlot);
                            if (fileIGT != -1)
                            {
                                creditsRolling = true;
                                localIGT = fileIGT;
                                return localIGT;
                            }
                        }

                        // if we aren't loaded but creditsRolling is true
                        // then it means credits are over. 
                        if (_isLoaded && creditsRolling)
                        {
                            creditsRolling = false;
                            previousNgCount = _tmpNgCount;
                            localIGT = _tmpIGT;
                            return localIGT;
                        }
                    } else
                    {
                        // game running normally
                        return _tmpIGT;
                    }
                }

                // IGT is always at 0 during quitouts
                if (_tmpIGT == 0)
                {
                    // fix the IGT only once per quitout
                    if (!quitoutLatch)
                    {
                        int fileIGT = DarkSouls.GetCurrentSlotIGT(DarkSouls.CurrentSaveSlot);
                        if (fileIGT != -1)
                        {
                            quitoutLatch = true;
                            localIGT = fileIGT;
                            return localIGT;
                        }
                    }
                }
            }

            // If not ready, return whatever IGT we had on previous update
            // Game may be closed
            return localIGT;
        }

        /// <summary>
        /// Hook event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DarkSoulsProcess_OnHooked(object sender, PHEventArgs e)
        {
            switch (Version)
            {
                case GameVersion.Remastered:
                    DarkSouls = new Remastered(this);
                    previousNgCount = DarkSouls.NgCount;
                    ngChanged = false;
                    break;
                case GameVersion.PrepareToDie:
                    DarkSouls = new PrepareToDie(this);
                    previousNgCount = DarkSouls.NgCount;
                    ngChanged = false;
                    break;
                default:
                    DarkSouls = null;
                    break;
            }
        }

        /// <summary>
        /// Unhook event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DarkSoulsProcess_OnUnhooked(object sender, PHEventArgs e)
        {
            DarkSouls = null;
            creditsRolling = false;
            ngChanged = false;
            quitoutLatch = true;
        }
    }
}
