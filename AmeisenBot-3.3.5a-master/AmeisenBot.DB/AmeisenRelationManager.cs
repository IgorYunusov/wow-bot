using AmeisenBotData;
using AmeisenBotPersistence.Models;
using AmeisenBotUtilities.Enums;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AmeisenBotPersistence
{
    public class AmeisenRelationManager
    {
        private AmeisenDataHolder AmeisenDataHolder { get; set; }
        private List<Relationship> Relationships { get; set; }

        public AmeisenRelationManager(AmeisenDataHolder ameisenDataHolder)
        {
            AmeisenDataHolder = ameisenDataHolder;
            LoadRelationships();
        }

        public void LoadRelationships()
        {
            string path = AmeisenDataHolder.Settings.relationshipsPath;

            // if no custom path is set, use the default .\data\NAME-relationships.json
            if (path == "")
            {
                if (!Directory.Exists(AmeisenGlobals.dataDir)) { Directory.CreateDirectory(AmeisenGlobals.dataDir); }
                path = $"{AmeisenGlobals.dataDir}{AmeisenDataHolder.Me.Name}-relationships.json";
            }

            if (File.Exists(path))
            {
                Relationships = JsonConvert.DeserializeObject<List<Relationship>>(File.ReadAllText(path));
            }
            else
            {
                Relationships = new List<Relationship>();
            }
        }

        public void SaveRelationships()
        {
            string path = AmeisenDataHolder.Settings.relationshipsPath;

            // if no custom path is set, use the default .\data\NAME-relationships.json
            if (path == "")
            {
                if (!Directory.Exists(AmeisenGlobals.dataDir)) { Directory.CreateDirectory(AmeisenGlobals.dataDir); }
                path = $"{AmeisenGlobals.dataDir}{AmeisenDataHolder.Me.Name}-relationships.json";
            }

            File.WriteAllText(path, JsonConvert.SerializeObject(Relationships));
        }

        /// <summary>
        /// Add or Update a relationship, already present relationships will be removed
        /// </summary>
        /// <param name="playerName">name of the player</param>
        /// <param name="relation">relation to that player</param>
        public void AddOrUpdateRelationship(string playerName, Relation relation)
        {
            Relationship relationship = new Relationship(playerName, relation);

            List<Relationship> relationships = Relationships.Where(r => r.PlayerName == playerName).ToList();
            int count = relationships.Count;

            if (count > 0)
            {
                foreach(Relationship r in relationships)
                {
                    Relationships.Remove(r);
                }
            }

            if (!Relationships.Contains(relationship)) { Relationships.Add(relationship); }
        }

        public Relation GetRelationToPlayer(string playerName)
        {
            List<Relationship> result = Relationships.Where(r => r.PlayerName == playerName).ToList();

            if (result.Count > 0)
            {
                return result.First().Relation;
            }

            return Relation.None;
        }
    }
}
