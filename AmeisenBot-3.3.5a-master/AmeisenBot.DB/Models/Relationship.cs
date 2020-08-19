using AmeisenBotUtilities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmeisenBotPersistence.Models
{
    public class Relationship
    {
        public string PlayerName { get; private set; }
        public Relation Relation { get; private set; }

        public Relationship(string playerName, Relation relation)
        {
            PlayerName = playerName;
            Relation = relation;
        }
    }
}
