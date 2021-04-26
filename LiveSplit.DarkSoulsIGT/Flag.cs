namespace LiveSplit.DarkSoulsIGT {
    public enum FlagTypes {
        Boss
    }

    public class Flag {
        public FlagTypes Type { get; private set; }
        public int FlagID { get; private set; }

        public Flag(FlagTypes type, int flagID)
        {
            this.Type = type;
            this.FlagID = flagID;
        }
    }

    public class BossFlag : Flag {
        public string Name { get; private set; }

        public BossFlag(string name, int flagID) : base(FlagTypes.Boss, flagID)
        {
            this.Name = name;
        }
    }

}
