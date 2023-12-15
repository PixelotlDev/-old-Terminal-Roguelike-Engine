namespace AdventureGame
{
    enum PlayerClass
    {
        fighter,
        thief,
        mage,
        acrobat
    }

    internal struct PlayerStats
    {
        public Statistic constitutionStat;
        public Statistic strengthStat;
        public Statistic dexterityStat;
        public Statistic agilityStat;
        public Statistic observationStat;
        public Statistic aimStat;
        public Statistic stealthStat;
        public Statistic magicStat;

        public PlayerStats(int con, int str, int dex, int agi, int obs, int aim, int ste, float mag)
        {
            constitutionStat = new(con);
            strengthStat = new(str);
            dexterityStat = new(dex);
            agilityStat = new(agi);
            observationStat = new(obs);
            aimStat = new(aim);
            stealthStat = new(ste);
            magicStat = new(mag);
        }
    }

    internal struct Statistic
    {
        private readonly float baseValue;
        private float addModifier = 0;
        private float multModifier = 1;

        public float AddModifier
        {
            private get { return this.addModifier; }
            set { this.addModifier = value; }
        }
        public float MultModifier
        {
            private get { return this.multModifier; }
            set { this.multModifier = value; }
        }

        public Statistic(float baseInt) { baseValue = baseInt; }

        public float GetNetValue() => (baseValue * multModifier) + addModifier;
    }
}
