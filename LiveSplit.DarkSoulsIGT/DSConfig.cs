namespace LiveSplit.DarkSoulsIGT
{
    internal enum GameVersion
    {
        PrepareToDie,
        Remastered,
    }

    internal static class DSConfig
    {
        public static string PrepareToDie = "DARKSOULS";
        public static string Remastered = "DarkSoulsRemastered";
        public static int MinLifeSpan = 5000; // 5 seconds
        public static string AOBNotFound = "Array of Bytes not found ({0})";
        public static string AOBMultiple = "Array of Bytes found {0} times instead of just 1 time. ({0})";
    }
}
