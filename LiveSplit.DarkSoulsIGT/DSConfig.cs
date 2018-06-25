using System;

namespace LiveSplit.DarkSoulsIGT
{
    internal enum GameVersion
    {
        Release,
        Debug,
        Unknown
    }

    internal static class DSConfig
    {
        public static string Module = "DARKSOULS";
        public static string AOBNotFound = "Array of Bytes not found ({0})";
        public static string AOBMultiple = "Array of Bytes found {0} times instead of just 1 time. ({0})";

        public static IntPtr VersionCheck = new IntPtr(0x400080);
        public static uint VersionRelease = 0xFC293654;
        public static uint VersionDebug = 0xCE9634B4;
        
        //
        //public static int OffsetSize = 4;
        public static IntPtr BasePointerRelease = new IntPtr(0x01378700);
        public static IntPtr BasePointerDebug = new IntPtr(0x137C8C0);
    }
}
