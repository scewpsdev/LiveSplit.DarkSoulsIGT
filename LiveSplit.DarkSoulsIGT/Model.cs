using System;
using System.Diagnostics;
using LiveSplit.ComponentUtil;

namespace LiveSplit.DarkSoulsIGT {
    public enum GameVersion {
        PrepareToDie,
        Remastered,
        None,
    }

    public class Model {
        /// <summary>
        /// Singleton
        /// </summary>
        private static readonly Lazy<Model> lazy = new Lazy<Model>(() => new Model());
        public static Model Instance => lazy.Value;
        private DarkSouls darksouls;

        private event OnHookedEventHandler OnHooked;
        public event OnDarkSoulsHookedEventHandler OnDarkSoulsHooked;
        public event OnDarkSoulsUnhookedEventHandler OnDarkSoulsUnhooked;

        // mirror of the dark souls events
        public event DarkSouls.OnInGameTimeChangedEventHandler OnInGameTimeChanged;
        public event DarkSouls.OnBossDiedEventHandler OnBossDied;
        public event DarkSouls.OnQuitoutEventHandler OnQuitout;

        private Model()
        {
            OnHooked += Model_OnHooked;
        }

        private void Model_OnHooked(Process proc)
        {
            if (proc == null) return;

            if (!ExtensionMethods.Is64Bit(proc))
            {
                darksouls = new PTDE(proc);
                darksouls.OnInGameTimeChanged += DarkSouls_OnInGameTimeChanged;
                darksouls.OnBossDied += Darksouls_OnBossDied;
                darksouls.OnQuitout += Darksouls_OnQuitout;
                OnDarkSoulsHooked?.Invoke(darksouls);
                proc.Exited += Proc_Exited;
            } 
            else
            {
                // @TODO create DSR
            }

        }

        private void DarkSouls_OnInGameTimeChanged(int old, int current)
        {
            OnInGameTimeChanged?.Invoke(old, current);
        }

        private void Darksouls_OnBossDied(BossFlag boss)
        {
            OnBossDied?.Invoke(boss);
        }

        private void Darksouls_OnQuitout()
        {
            OnQuitout?.Invoke();
        }

        private void Proc_Exited(object sender, EventArgs e)
        {
            darksouls = null;
            OnDarkSoulsUnhooked?.Invoke();
        }

        /// <summary>
        /// Find the game process and call events to notify hooking happened
        /// </summary>
        private void Hook()
        {
            var candidates = Process.GetProcessesByName("DARKSOULS");
            Process candidate = (candidates.Length > 0) ? candidates[0] : null;

            if (candidate != null)
            {
                OnHooked?.Invoke(candidate);
            }
        }

        public void Update()
        {
            if (darksouls == null)
            {
                Hook();
            }

            darksouls?.Update();
        }

        public void Reset()
        {
            darksouls?.Reset();
        }

        public void ResetIndexes()
        {
            darksouls?.ResetIndexes();
        }

        public delegate void OnHookedEventHandler(Process proc);
        public delegate void OnDarkSoulsHookedEventHandler(DarkSouls game);
        public delegate void OnDarkSoulsUnhookedEventHandler();


    }

    //class Model : PHook
    //{
    //    /// <summary>
    //    /// Constants
    //    /// </summary>
    //    private const string PTDE_NAME = "DARKSOULS";
    //    private const string REMASTERED_NAME = "DARK SOULS™: REMASTERED";
    //    private const int REFRESH_INTERVAL = 5000;
    //    private const int MIN_LIFE_SPAN = 5000;

    //    /// <summary>
    //    /// Local private variables
    //    /// </summary>
    //    private int localIGT;
    //    private bool quitoutLatch;
    //    private bool unhookedLatch;
    //    private bool creditsRolling;
    //    private int previousCurrentSaveSlot;
    //    private int previousNgCount;
    //    private string savefilePath;

    //    /// <summary>
    //    /// Process Selector
    //    /// </summary>
    //    private static Func<Process, bool> processSelector = (p) =>
    //    {
    //        return (p.MainWindowTitle == REMASTERED_NAME) || (p.ProcessName == PTDE_NAME);
    //    };

    //    /// <summary>
    //    /// Abstract DarkSouls class
    //    /// Could be null
    //    /// </summary>
    //    private DarkSouls DarkSouls { get; set; }

    //    /// <summary>
    //    /// If the model is ready to be used or not
    //    /// </summary>
    //    public bool Ready
    //    {
    //        get => Hooked && DarkSouls != null;
    //    }

    //    /// <summary>
    //    /// Game version. Only remastered is 64 bits
    //    /// </summary>
    //    public GameVersion Version
    //    {
    //        get
    //        {
    //            return (Is64Bit) ? GameVersion.Remastered : GameVersion.PrepareToDie;
    //        }
    //    }

    //    /// <summary>
    //    /// Constructor
    //    /// </summary>
    //    public Model() : base(REFRESH_INTERVAL, MIN_LIFE_SPAN, processSelector)
    //    {
    //        OnHooked += DarkSoulsProcess_OnHooked;
    //        OnUnhooked += DarkSoulsProcess_OnUnhooked;
    //    }

    //    /// <summary>
    //    /// Dispose
    //    /// </summary>
    //    public void Dispose()
    //    {
    //        if (Process != null)
    //        {
    //            Process.Dispose();
    //        }
    //    }

    //    /// <summary>
    //    /// Reset some variables when needed
    //    /// </summary>
    //    public void Reset()
    //    {
    //        localIGT = 0;
    //        quitoutLatch = true;
    //        creditsRolling = false;
    //        unhookedLatch = false;
    //        previousNgCount = int.MaxValue;
    //    }

    //    /// <summary>
    //    /// Resets the inventory indexes if possible
    //    /// </summary>
    //    public void ResetIndexes()
    //    {
    //        if (Ready)
    //        {
    //            DarkSouls.ResetInventory();
    //        }
    //    }

    //    public int GetMemoryInGameTime()
    //    {

    //        return (Ready) ? DarkSouls.MemoryIGT : 0;
    //    }

    //    /// <summary>
    //    /// Returns the IGT
    //    /// Reads the savefile's IGT on quitouts and credits
    //    /// Reads the memory IGT otherwise
    //    /// </summary>
    //    public int GetInGameTime()
    //    {
    //        if (Ready)
    //        {
    //            unhookedLatch = false;
    //            int IGT = -1;

    //            try
    //            {
    //                // Read the Dark Souls process once at the start to
    //                // avoid the value
    //                int _tmpIGT = DarkSouls.MemoryIGT;
    //                int _tmpNgCount = DarkSouls.NgCount;
    //                int _tmpCurrentSaveSlot = DarkSouls.CurrentSaveSlot;
    //                bool _isLoaded = DarkSouls.Loaded;
    //                string _tmpSavefilePath = DarkSouls.GetSaveFileLocation();

    //                // If IGT is running
    //                if (_tmpIGT > 0)
    //                {
    //                    // then we're not on a quitout
    //                    quitoutLatch = false;

    //                    // Check if NG increased since last update (in-game cutscene is playing)
    //                    if (_tmpNgCount > previousNgCount)
    //                    {
    //                        // Final in-game cutscene is playing, just keep using memory IGT
    //                        if (_isLoaded && !creditsRolling)
    //                        {
    //                            IGT = _tmpIGT;
    //                        }

    //                        // if the player isn't loaded, then credits are playing (video cutscene is playing)
    //                        if (!_isLoaded && !creditsRolling)
    //                        {
    //                            int fileIGT = SL2Reader.GetCurrentSlotIGT(_tmpSavefilePath, previousCurrentSaveSlot);
    //                            if (fileIGT != -1)
    //                            {
    //                                creditsRolling = true;
    //                                savefilePath = _tmpSavefilePath;
    //                                IGT = fileIGT;
    //                            }
    //                            else
    //                            {
    //                                // reading the file failed for some reason, use
    //                                // memory IGT. Also don't setup the latch
    //                                IGT = _tmpIGT;
    //                            }
    //                        }

    //                        // if we aren't loaded but creditsRolling is true
    //                        // then it means credits are over. 
    //                        if (_isLoaded && creditsRolling)
    //                        {
    //                            creditsRolling = false;
    //                            previousNgCount = _tmpNgCount; // only update previousNgCount onces credits are over
    //                            IGT = _tmpIGT;
    //                        }
    //                    }
    //                    else
    //                    {
    //                        // game running normally
    //                        // update previous variables for next update
    //                        previousNgCount = _tmpNgCount;
    //                        previousCurrentSaveSlot = _tmpCurrentSaveSlot;
    //                        IGT = _tmpIGT;
    //                    }
    //                }

    //                // IGT is always at 0 during quitouts
    //                if (_tmpIGT == 0)
    //                {
    //                    // fix the IGT only once per quitout
    //                    if (!quitoutLatch)
    //                    {
    //                        quitoutLatch = true;

    //                        int fileIGT = SL2Reader.GetCurrentSlotIGT(_tmpSavefilePath, previousCurrentSaveSlot);
    //                        if (fileIGT != -1)
    //                        {
    //                            savefilePath = _tmpSavefilePath;
    //                            IGT = fileIGT;
    //                        }
    //                        else
    //                        {
    //                            // reading the file failed for some reason, use
    //                            // the old rollback method. Also don't setup the latch
    //                            localIGT -= DarkSouls.QuitoutRollback;
    //                        }
    //                    }
    //                }
    //            }
    //            catch
    //            {
    //                IGT = -1;
    //            }

    //            // computation of IGT worked
    //            // and game is still ready
    //            if (IGT != -1 && Ready)
    //            {
    //                localIGT = IGT;
    //            }
    //        } else
    //        {
    //            // If game is closed (FQ or crash) and timer was running, return IGT from savefile
    //            if (localIGT > 0 && !unhookedLatch)
    //            {
    //                unhookedLatch = true;

    //                int fileIGT = SL2Reader.GetCurrentSlotIGT(savefilePath, previousCurrentSaveSlot);
    //                if (fileIGT != -1)
    //                {
    //                    localIGT = fileIGT;
    //                }
    //            }
    //        }

    //        return localIGT;
    //    }

    //    /// <summary>
    //    /// Hook event
    //    /// </summary>
    //    /// <param name="sender"></param>
    //    /// <param name="e"></param>
    //    private void DarkSoulsProcess_OnHooked(object sender, PHEventArgs e)
    //    {
    //        switch (Version)
    //        {
    //            case GameVersion.Remastered:
    //                DarkSouls = new Remastered(this);
    //                break;
    //            case GameVersion.PrepareToDie:
    //                DarkSouls = new PrepareToDie(this);
    //                break;
    //            default:
    //                DarkSouls = null;
    //                break;
    //        }

    //        if (Ready)
    //        {
    //            previousNgCount = DarkSouls.NgCount;
    //            quitoutLatch = true;
    //            creditsRolling = false;
    //        }
    //    }

    //    /// <summary>
    //    /// Unhook event
    //    /// </summary>
    //    /// <param name="sender"></param>
    //    /// <param name="e"></param>
    //    private void DarkSoulsProcess_OnUnhooked(object sender, PHEventArgs e)
    //    {
    //        DarkSouls = null;
    //        creditsRolling = false;
    //        quitoutLatch = true;
    //    }
    //}
}
