using System;
using System.Collections.Generic;

namespace LiveSplit.DarkSoulsIGT
{
    internal static class DSIGTConfig
    {
        public static Dictionary<GameVersion, int> RefreshRate = new Dictionary<GameVersion, int>
        {
            { GameVersion.PrepareToDie, 33 },
            { GameVersion.Remastered, 16 },
        };

        public static Dictionary<GameVersion, int> TimerQuitoutDelay = new Dictionary<GameVersion, int>
        {
            { GameVersion.PrepareToDie, 594 },
            { GameVersion.Remastered, 512 },
        };

        public static Dictionary<GameVersion, int> Offset = new Dictionary<GameVersion, int>
        {
            { GameVersion.PrepareToDie, 0x68 },
            { GameVersion.Remastered, 0xA4 },
        };

        public static Dictionary<GameVersion, int> ArrayOfByteOffset = new Dictionary<GameVersion, int>
        {
            { GameVersion.PrepareToDie, 2 },
            { GameVersion.Remastered, 3 },
        };

        public static Dictionary<GameVersion, byte?[]> ArrayOfBytes = new Dictionary<GameVersion, byte?[]>
        {
            {
                GameVersion.PrepareToDie,
                new byte?[]
                {
                    0x8B, 0x0D, null, null, null, null, 0x8B ,0x7E ,0x1C, 0x8B, 0x49, 0x08, 0x8B, 0x46, 0x20, 0x81, 0xC1, 0xB8, 0x01, 0x00, 0x00, 0x57, 0x51, 0x32, 0xDB
                }
            },
            {
                GameVersion.Remastered,
                new byte?[]
                {
                    0x48, 0x8B, 0x05, null, null, null, null, 0x32, 0xDB, 0x48, 0x8B, 0x48, 0x10, 0x48, 0x81, 0xC1, 0x80, 0x02, 0x00, 0x00
                }
            },
        };
    }
}
