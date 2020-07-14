using System;
using PropertyHook;

namespace LiveSplit.DarkSoulsIGT
{
    public abstract class DarkSouls
    {
        /// <summary>
        /// Local private variables
        /// </summary>
        private int EQUIPMENT_SLOT_COUNT = 20;

        /// <summary>
        /// The Game. Either Dark Souls or Remastered
        /// </summary>
        public PHook Process { get; set; }

        /// <summary>
        /// Pointers to CharClassBase
        /// </summary>
        public PHPointer pCharClassBase { get; set; }

        /// <summary>
        /// Pointers to inventory indexes
        /// </summary>
        public PHPointer pInventoryReset { get; set; }

        /// <summary>
        /// Pointers to current slot index
        /// </summary>
        public PHPointer pCurrentSlot { get; set; }

        /// <summary>
        /// Loaded pointer
        /// </summary>
        public PHPointer pLoaded { get; set; }

        /// <summary>
        /// Returns if the player's game is current loaded (aka in game)
        /// </summary>
        public bool Loaded
        {
            get
            {
                return pLoaded.Resolve() != IntPtr.Zero;
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public DarkSouls(PHook process)
        {
            this.Process = process;
        }

        /// <summary>0
        /// Reset the inventory indexes
        /// </summary>
        public void ResetInventory()
        {
            for (int i = 0; i < EQUIPMENT_SLOT_COUNT; i++)
            {
                pInventoryReset.WriteUInt32(0x4 * i, UInt32.MaxValue);
            }
        }

        /// <summary>
        /// Returns the game savefile path
        /// </summary>
        /// <returns></returns>
        public abstract string GetSaveFileLocation();

        /// <summary>
        /// Rollback value, used as a fallback
        /// </summary>
        public abstract int QuitoutRollback
        {
            get;
        }

        /// <summary>
        /// Returns the raw IGT from Memory
        /// </summary>
        /// <returns></returns>
        public abstract int MemoryIGT
        {
            get;
        }

        /// <summary>
        /// Current Save Slot index
        /// </summary>
        public abstract int CurrentSaveSlot
        {
            get;
        }

        /// <summary>
        /// Current new game counter
        /// </summary>
        public abstract int NgCount
        {
            get;
        }
    }
}
