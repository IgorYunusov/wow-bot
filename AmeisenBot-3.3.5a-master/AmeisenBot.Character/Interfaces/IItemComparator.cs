using AmeisenBot.Character.Objects;

namespace AmeisenBot.Character.Interfaces
{
    public interface IItemComparator
    {
        /// <summary>
        /// Return true if you should select a over b
        /// </summary>
        /// <param name="a">Item a</param>
        /// <param name="b">Item b</param>
        /// <returns>wether you should select item a or not</returns>
        bool Compare(Item a, Item b);
    }
}