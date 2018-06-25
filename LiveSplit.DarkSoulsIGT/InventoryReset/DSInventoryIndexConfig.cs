using System;

namespace LiveSplit.DarkSoulsIGT
{
    internal static class DSInventoryIndexConfig
    {
        public static int ArrayOfByteOffset = 36;
        public static byte?[] ArrayOfBytes = new byte?[]
        {
            0x8B, 0x4C, 0x24, 0x34, 0x8B, 0x44, 0x24, 0x2C, 0x89, 0x8A, 0x38, 0x01, 0x00, 0x00, 0x8B, 0x90, 0x08, 0x01, 0x00, 0x00, 0xC1, 0xE2, 0x10, 0x0B, 0x90, 0x00, 0x01, 0x00, 0x00, 0x8B, 0xC1, 0x8B, 0xCD, 0x89, 0x14, 0xAD, null, null, null, null
        };

        public static IntPtr[] GetInventoryAddresses(IntPtr baseAddress)
        {
            return new IntPtr[]
            {
                baseAddress,            // slot 7
                baseAddress+0x4,        // slot 0
                baseAddress+0x8,        // slot 8
                baseAddress+0xC,        // slot 1
                baseAddress+0x3C,       // slot 2
                baseAddress+0x3C+0x4,   // slot 3
                baseAddress+0x3C+0x8,   // slot 4
                baseAddress+0x3C+0xC,   // slot 5
                baseAddress+0x3C+0x10,  // slot 6
                baseAddress+0x20,       // slot 14
                baseAddress+0x20+0x4,   // slot 15
                baseAddress+0x20+0x8,   // slot 16
                baseAddress+0x20+0xC,   // slot 17
                baseAddress+0x34,       // slot 18
                baseAddress+0x34+0x4,   // slot 19
                baseAddress+0x10,       // slot 9
                baseAddress+0x10+0x8,   // slot 10
                baseAddress+0x14,       // slot 11
                baseAddress+0x14+0x8,   // slot 12
            };
        }
    }
}
