using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace LiveSplit.DarkSoulsIGT
{
    static class GameInfo
    {
        internal static readonly string[] bossNames =
        {
            "Asylum Demon",
            "Taurus Demon",
            "Bell Gargoyles",
            "Capra Demon",
            "Gaping Dragon",
            "Iron Golem",
            "Ornstein and Smough",
            "Chaos Witch Quelaag",
            "Ceaseless Discharge",
            "Firesage Demon",
            "Centipede Demon",
            "Bed of Chaos",
            "Pinwheel",
            "Gravelord Nito",
            "Four Kings",
            "Great Grey Wolf Sif",
            "Moonlight Butterfly",
            "Seath the Scaleless",
            "Stray Demon",
            "Dark Sun Gwyndolin",
            "Crossbreed Priscilla",
            "Gwyn Lord of Cinder",
            "Sanctuary Guardian",
            "Artorias the Abysswalker",
            "Manus, Father of the Abyss",
            "Black Dragon Kalameet"
        };

        // TODO(scewps): complete this list
        internal static readonly string[] itemTypes =
        {
            "Melee Weapons",
            "Rings",
        };

        // TODO(scewps): complete this list
        internal static readonly string[] rings =
        {
            "Red Tearstone Ring",
        };


        internal static string[] GetItemNames(int itemType)
        {
            switch (itemType)
            {
                case 0: // Melee Weapons
                    return new string[] { };
                case 1: // Rings
                    return rings;
                default:
                    Debug.Assert(false);
                    return null;
            }
        }
    }
}
