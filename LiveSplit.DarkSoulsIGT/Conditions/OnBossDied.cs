namespace LiveSplit.DarkSoulsIGT.Conditions {
    class OnBossDied: Condition {
        private BossFlag boss;
        private bool isDead = false;

        public OnBossDied(BossFlag boss): base()
        {
            this.boss = boss;
        }

        public override void Reset()
        {
            isDead = false;
        }

        public override void Start()
        {
            Model.Instance.OnBossDied += Instance_OnBossDied;
        }

        public override void Stop()
        {
            Model.Instance.OnBossDied -= Instance_OnBossDied;
        }

        private void Instance_OnBossDied(BossFlag boss)
        {
            if (boss.FlagID != this.boss.FlagID) return;
            if (isDead) return;

            isDead = true;
            RaiseOnConditionComplete();
            Stop();
        }

        public static BossFlag[] Options => Flags.Bosses;
    }
}
