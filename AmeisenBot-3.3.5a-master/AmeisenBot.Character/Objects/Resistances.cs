namespace AmeisenBot.Character.Objects
{
    public class Resistances
    {
        public Resistances()
        {
            Update();
        }

        public double Arcane { get; set; }
        public double Fire { get; set; }
        public double Frost { get; set; }
        public double Nature { get; set; }
        public double Shadow { get; set; }

        public void Update()
        {
            // TODO: Read the resistances
        }
    }
}