using AmeisenBot.Clients;
using AmeisenBotCore;
using AmeisenBotData;
using AmeisenBotPersistence;
using AmeisenBotUtilities;
using System.Threading;

namespace AmeisenBotFSM.Actions
{
    public class ActionLoot : ActionMoving
    {
        public ActionLoot(
            AmeisenDataHolder ameisenDataHolder,
            AmeisenDBManager ameisenDBManager,
            AmeisenNavmeshClient ameisenNavmeshClient) : base(ameisenDataHolder, ameisenDBManager, ameisenNavmeshClient)
        {
            AmeisenDataHolder = ameisenDataHolder;
        }

        public override void DoThings()
        {
            if (WaypointQueue.Count > 0)
            {
                base.DoThings();
            }

            Unit unitToLoot = AmeisenDataHolder.LootableUnits.Peek();
            double distance = Utils.GetDistance(Me.pos, unitToLoot.pos);

            if (distance > 3)
            {
                if (!WaypointQueue.Contains(unitToLoot.pos))
                {
                    WaypointQueue.Enqueue(unitToLoot.pos);
                }
            }
            else
            {
                AmeisenCore.TargetGUID(unitToLoot.Guid);
                AmeisenCore.LuaDoString("InteractUnit(\"target\");");
                Thread.Sleep(1000);
                // We should have looted it by now based on the event
                AmeisenDataHolder.LootableUnits.Dequeue();
            }
        }

        private AmeisenDataHolder AmeisenDataHolder { get; set; }

        private Me Me
        {
            get { return AmeisenDataHolder.Me; }
            set { AmeisenDataHolder.Me = value; }
        }
    }
}