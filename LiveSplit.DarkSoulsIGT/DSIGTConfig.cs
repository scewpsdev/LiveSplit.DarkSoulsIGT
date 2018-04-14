using System;

namespace LiveSplit.DarkSoulsIGT
{
    internal static class DSIGTConfig
    {
        public static string Module = "DARKSOULS";
        public const int TimerQuitoutDelay = 594;

        public static IntPtr VersionCheck = new IntPtr(0x400080);
        public static uint VersionRelease = 0xFC293654;
        public static uint VersionDebug = 0xCE9634B4;

        public static int Offsets = 0x68;
        public static int OffsetSize = 4;
        public static IntPtr BasePointerRelease = new IntPtr(0x01378700);
        public static IntPtr BasePointerDebug = new IntPtr(0x137C8C0);

        public const int CharData1Ptr = 0x137DC70;
        public const int CharData1Ptr2 = 0x4;
        public const int CharData1Ptr3 = 0x0;
    }
}
