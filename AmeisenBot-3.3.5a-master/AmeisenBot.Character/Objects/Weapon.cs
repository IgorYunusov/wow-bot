namespace AmeisenBot.Character.Objects
{
    public class Weapon : Item
    {
        public Weapon(int slot) : base(slot)
        {
            DamageStats = DamageStats.Update();
        }

        private DamageStats DamageStats { get; set; }
    }
}