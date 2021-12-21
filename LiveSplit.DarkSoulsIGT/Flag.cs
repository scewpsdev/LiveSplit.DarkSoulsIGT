using System;

namespace LiveSplit.DarkSoulsIGT
{
    public enum FlagTypes
    {
        Boss,
        Item,
    }

    public class Flag
    {
        public FlagTypes Type { get; private set; }
        public int FlagID { get; private set; }

        public Flag(FlagTypes type, int flagID)
        {
            this.Type = type;
            this.FlagID = flagID;
        }
    }

    public class BossFlag : Flag
    {
        public string Name { get; private set; }

        public BossFlag(string name, int flagID) : base(FlagTypes.Boss, flagID)
        {
            this.Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class ItemFlag : Flag
    {
        public string name;

        public ItemFlag(string name, int flagID) : base(FlagTypes.Item, flagID)
        {
            this.name = name;
        }

        public override string ToString()
        {
            return name;
        }
    }
}
