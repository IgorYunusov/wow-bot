namespace AmeisenBot.Character.Objects
{
    public class SecondaryStats
    {
        public SecondaryStats()
        {
            Update();
        }

        public double BlockRating { get; set; }
        public double CritRating { get; set; }
        public double EvadeRating { get; set; }
        public double HitRating { get; set; }
        public double Resilience { get; set; }

        public void Update()
        {
            // TODO: Read the secondary stats
        }
    }
}