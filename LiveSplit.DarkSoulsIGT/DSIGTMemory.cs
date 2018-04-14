using System;
using System.Runtime.InteropServices;

namespace LiveSplit.DarkSoulsIGT
{
    internal static class DSIGTMemory
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        public static Int32 RInt32(IntPtr HANDLE, IntPtr addr)
        {
            int bytesRead = 0;
            byte[] _rtnBytes = new byte[4];
            ReadProcessMemory(HANDLE, addr, _rtnBytes, _rtnBytes.Length, ref bytesRead);
            return BitConverter.ToInt32(_rtnBytes, 0);
        }

        public static UInt32 RUInt32(IntPtr HANDLE, IntPtr addr)
        {
            int bytesRead = 0;
            byte[] _rtnBytes = new byte[4];
            ReadProcessMemory(HANDLE, addr, _rtnBytes, _rtnBytes.Length, ref bytesRead);
            return BitConverter.ToUInt32(_rtnBytes, 0);
        }

        public static Int32 RInt32(IntPtr HANDLE, int addr)
        {
            return RInt32(HANDLE, (IntPtr)addr);
        }
    }
}
