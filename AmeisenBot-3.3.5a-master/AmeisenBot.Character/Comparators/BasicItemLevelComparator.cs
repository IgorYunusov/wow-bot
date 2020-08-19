using AmeisenBot.Character.Interfaces;
using AmeisenBot.Character.Objects;

namespace AmeisenBot.Character.Comparators
{
    public class BasicItemLevelComparator : IItemComparator
    {
        /// <summary>
        /// Very basic Itemlevel Comporator, only outputs true
        /// if the item a is higher than b
        /// </summary>
        /// <param name="a">Item a</param>
        /// <param name="b">Item b</param>
        /// <returns>wether you should equip item a or not</returns>
        public bool Compare(Item a, Item b)
            => a.Level > b.Level;
    }
}