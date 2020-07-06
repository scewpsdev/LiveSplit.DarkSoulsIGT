using System;
using PropertyHook;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LiveSplit.DarkSoulsIGT
{
    public abstract class DarkSouls
    {
        /// <summary>
        /// Local private variables
        /// </summary>
        private int[] InventoryOffsets = new int[]
        {
            0,          // slot 7
            0x4,        // slot 0
            0x8,        // slot 8
            0xC,        // slot 1
            0x3C,       // slot 2
            0x3C+0x4,   // slot 3
            0x3C+0x8,   // slot 4
            0x3C+0xC,   // slot 5
            0x3C+0x10,  // slot 6
            0x20,       // slot 14
            0x20+0x4,   // slot 15
            0x20+0x8,   // slot 16
            0x20+0xC,   // slot 17
            0x34,       // slot 18
            0x34+0x4,   // slot 19
            0x10,       // slot 9
            0x10+0x8,   // slot 10
            0x14,       // slot 11
            0x14+0x8,   // slot 12
        };

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

        /// <summary>
        /// Reset the inventory indexes
        /// </summary>
        public void ResetInventory()
        {
            foreach (int offset in InventoryOffsets)
            {
                pInventoryReset.WriteUInt32(offset, UInt32.MaxValue);
            }
        }

        /// <summary>
        /// Returns the IGT of a specific slot in the game's savefile
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        public abstract int GetCurrentSlotIGT(int slot = 0);

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
