using AmeisenBot.Clients;
using AmeisenBotCore;
using AmeisenBotData;
using AmeisenBotPersistence;
using AmeisenBotUtilities;
using System.Collections.Generic;
using static AmeisenBotFSM.Objects.Delegates;

namespace AmeisenBotFSM.Actions
{
    public class ActionDead : ActionMoving
    {
        public ActionDead(
            AmeisenDataHolder ameisenDataHolder,
            AmeisenDBManager ameisenDBManager,
            AmeisenNavmeshClient ameisenNavmeshClient) : base(ameisenDataHolder, ameisenDBManager, ameisenNavmeshClient)
        {
            AmeisenDataHolder = ameisenDataHolder;
            AmeisenDBManager = ameisenDBManager;

            // going to save these in a databse sometime
            InstanceEntrances = new Dictionary<Vector3, Vector3>
            {
                { new Vector3(5776, 2065, -500), new Vector3(5778, 2062, 636) } // Icecrown Citadel
            };
        }

        public override Start StartAction { get { return Start; } }
        public override DoThings StartDoThings { get { return DoThings; } }
        public override Exit StartExit { get { return Stop; } }

        public override void DoThings()
        {
            GoToCorpseAndRevive();
            base.DoThings();
        }

        public override void Start() => base.Start();

        public override void Stop() => base.Stop();

        private Unit ActiveUnit { get; set; }
        private List<Unit> ActiveUnits { get; set; }
        private AmeisenDataHolder AmeisenDataHolder { get; set; }
        private AmeisenDBManager AmeisenDBManager { get; set; }

        private Dictionary<Vector3, Vector3> InstanceEntrances { get; set; }

        private Me Me
        {
            get { return AmeisenDataHolder.Me; }
            set { AmeisenDataHolder.Me = value; }
        }

        private void GoToCorpseAndRevive()
        {
            Vector3 corpsePosition = AmeisenCore.GetCorpsePosition();
            corpsePosition.X = (int)corpsePosition.X;
            corpsePosition.Y = (int)corpsePosition.Y;
            corpsePosition.Z = (int)corpsePosition.Z;

            if (InstanceEntrances.ContainsKey(corpsePosition))
            {
                // Dungeon/Raid workaround
                // because blizzard decided to put the corpse at very low Z axis values that
                // we cant navigate to them, so we are going to use the position of the entrance instead
                corpsePosition = InstanceEntrances[corpsePosition];
            }

            if (corpsePosition.X != 0 && corpsePosition.Y != 0 && corpsePosition.Z != 0)
            {
                if (!WaypointQueue.Contains(corpsePosition))
                {
                    WaypointQueue.Enqueue(corpsePosition);
                }
            }

            if (Utils.GetDistance(Me.pos, corpsePosition) < 4.0)
            {
                AmeisenCore.RetrieveCorpse(true);
            }
        }
    }
}