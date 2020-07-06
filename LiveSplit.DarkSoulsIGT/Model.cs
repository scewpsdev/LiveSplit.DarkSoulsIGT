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
        /// Reset some variables when needed
        /// </summary>
        public void Reset()
        {
            localIGT = 0;
            quitoutLatch = true;
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
        /// Returns the IGT with pauses on quitouts
        /// </summary>
        public int GetInGameTime()
        {
            if (Ready)
            {
                int _tmpIGT = DarkSouls.MemoryIGT;
                int _tmpNgCount = DarkSouls.NgCount;

                // Check if credits just started rolling
                if (!ngChanged && !creditsRolling)
                {
                    // If game was beaten
                    if (_tmpNgCount > previousNgCount)
                    {
                        ngChanged = true;
                    }
                }

                // Update IGT normally with the latch logic
                // If not in the main menu, update the timer normally
                if (_tmpIGT > 0 && !creditsRolling)
                {
                    // Update IGT normally
                    localIGT = _tmpIGT;
                    quitoutLatch = false;
                }

                if (_tmpIGT > 0 && ngChanged)
                {
                    if (!DarkSouls.Loaded)
                    {
                        // When credits are rolling, to be consistent with ALTF4
                        // we read the IGT from the savefile instead of the memory
                        localIGT = DarkSouls.GetCurrentSlotIGT(DarkSouls.CurrentSaveSlot);
                        creditsRolling = true;
                    }

                    if (creditsRolling && DarkSouls.Loaded)
                    {
                        creditsRolling = false;
                        ngChanged = false;
                    }
                }

                // If in the game menu...
                if (_tmpIGT == 0 && !quitoutLatch)
                {
                    // When quitout, the IGT in memory is 0 so we read IGT from
                    // the savefile instead
                    localIGT = DarkSouls.GetCurrentSlotIGT(DarkSouls.CurrentSaveSlot);
                    quitoutLatch = true;
                }
            }

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
