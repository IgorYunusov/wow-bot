using AmeisenBot.Character.Structs;

namespace AmeisenBot.Character.Objects
{
    public class Spell
    {
        public Spell(string spellBookName, int spellbookId, string name, string rank, int castTime, int minRange, int maxRange, int costs)
        {
            SpellbookName = spellBookName;
            SpellbookId = spellbookId;
            Name = name;
            Rank = rank;
            CastTime = castTime;
            MinRange = minRange;
            MaxRange = maxRange;
            Costs = costs;
        }

        public Spell(RawSpell rawSpell)
        {
            SpellbookName = rawSpell.spellBookName;
            SpellbookId = rawSpell.spellbookId;
            Name = rawSpell.name;
            Rank = rawSpell.rank;
            CastTime = rawSpell.castTime;
            MinRange = rawSpell.minRange;
            MaxRange = rawSpell.maxRange;
            Costs = rawSpell.costs;
        }

        public int CastTime { get; private set; }
        public int Costs { get; private set; }
        public int MaxRange { get; private set; }
        public int MinRange { get; private set; }
        public string Name { get; private set; }
        public string Rank { get; private set; }
        public int SpellbookId { get; private set; }
        public string SpellbookName { get; private set; }

        public override string ToString()
        {
            return $"[{SpellbookName}] ({Rank}) [{Name}] Cost: {Costs} Range: {MinRange} - {MaxRange}m";
        }
    }
}