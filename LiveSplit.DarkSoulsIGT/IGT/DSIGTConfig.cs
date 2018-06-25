using System;
using System.Collections.Generic;

namespace LiveSplit.DarkSoulsIGT
{
    internal static class DSIGTConfig
    {
        public static int IGTRefreshRate = 33;
        public const int TimerQuitoutDelay = 594;
        public static int Offset = 0x68;
        public static int ArrayOfByteOffset = 2;
        public static byte?[] ArrayOfBytes = new byte?[]
        {
            0x8B, 0x0D, null, null, null, null, 0x8B ,0x7E ,0x1C, 0x8B, 0x49, 0x08, 0x8B, 0x46, 0x20, 0x81, 0xC1, 0xB8, 0x01, 0x00, 0x00, 0x57, 0x51, 0x32, 0xDB
        };

        public static Dictionary<GameVersion, IntPtr> IGTAddresses = new Dictionary<GameVersion, IntPtr>
        {
            { GameVersion.Release, new IntPtr(0x01378700) },
            { GameVersion.Debug, new IntPtr(0x137C8C0) },
        };
    }
}
