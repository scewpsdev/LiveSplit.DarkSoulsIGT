using System;
using System.Collections.Generic;

namespace LiveSplit.DarkSoulsIGT
{
    public static class Flags
    {
        private static readonly Dictionary<string, int> groups = new Dictionary<string, int>()
        {
            {"0", 0x00000},
            {"1", 0x00500},
            {"5", 0x05F00},
            {"6", 0x0B900},
            {"7", 0x11300},
        };

        private static readonly Dictionary<string, int> areas = new Dictionary<string, int>()
        {
            {"000", 00},
            {"100", 01},
            {"101", 02},
            {"102", 03},
            {"110", 04},
            {"120", 05},
            {"121", 06},
            {"130", 07},
            {"131", 08},
            {"132", 09},
            {"140", 10},
            {"141", 11},
            {"150", 12},
            {"151", 13},
            {"160", 14},
            {"170", 15},
            {"180", 16},
            {"181", 17},
        };

        /// <summary>
        /// Flag method that calcuates the offset according to the flag ID
        /// This is literally magic 
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        public static int GetEventFlagOffset(int ID, out uint mask)
        {
            string idString = ID.ToString("D8");

            if (idString.Length == 8)
            {
                string group = idString.Substring(0, 1);
                string area = idString.Substring(1, 3);
                int section = Int32.Parse(idString.Substring(4, 1));
                int number = Int32.Parse(idString.Substring(5, 3));

                if (Flags.groups.ContainsKey(group) && Flags.areas.ContainsKey(area))
                {
                    int offset = Flags.groups[group];
                    offset += Flags.areas[area] * 0x500;
                    offset += section * 128;
                    offset += (number - (number % 32)) / 8;

                    mask = 0x80000000 >> (number % 32);
                    return offset;
                }
            }

            throw new ArgumentException("Unknown event flag ID: " + ID);
        }

        static public BossFlag[] Bosses =
        {
            new BossFlag("Gaping Dragon", 2),
            new BossFlag("Bell Gargoyles", 3),
            new BossFlag("Priscilla", 4),
            new BossFlag("Sif", 5),
            new BossFlag("Pinwheel", 6),
            new BossFlag("Nito", 7),
            new BossFlag("Chaos Witch Quelaag", 9),
            new BossFlag("Bed of Chaos", 10),
            new BossFlag("Iron Golem", 11),
            new BossFlag("Ornstein & Smough", 12),
            new BossFlag("Four Kings", 13),
            new BossFlag("Seath", 14),
            new BossFlag("Gwyn", 15),
            new BossFlag("Asylum Demon", 16),
            new BossFlag("Taurus Demon", 11010901),
            new BossFlag("Capra Demon", 11010902),
            new BossFlag("Moonlight Butterfly", 11200900),
            new BossFlag("Sanctuary Guardian", 11210000),
            new BossFlag("Artorias", 11210001),
            new BossFlag("Manus", 11210002),
            new BossFlag("Kalameet", 11210004),
            new BossFlag("Demon Firesage", 11410410),
            new BossFlag("Ceaseless Discharge", 11410900),
            new BossFlag("Centipede Demon", 11410901),
            new BossFlag("Gwyndolin", 11510900),
            new BossFlag("Stray Demon", 11810900),
        };

        // TODO: complete this list
        public static string[] ItemTypes =
        {
            "Melee Weapons",
            "Rings",
        };

        // TODO: complete this list and set appropriate flags
        public static ItemFlag[] TestItems =
        {
            new ItemFlag("Red Tearstone Ring", 0),
        };
    }
}
