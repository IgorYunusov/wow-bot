using AmeisenBot.Clients;
using AmeisenBotData;
using AmeisenBotPersistence;
using AmeisenBotUtilities;
using AmeisenMovement;
using AmeisenMovement.Structs;
using System;
using System.Threading;
using static AmeisenBotFSM.Objects.Delegates;

namespace AmeisenBotFSM.Actions
{
    public class ActionFollow : ActionMoving
    {
        public ActionFollow(
            AmeisenDataHolder ameisenDataHolder,
            AmeisenDBManager ameisenDBManager,
            AmeisenMovementEngine ameisenMovementEngine,
            AmeisenNavmeshClient ameisenNavmeshClient) : base(ameisenDataHolder, ameisenDBManager, ameisenNavmeshClient)
        {
            AmeisenDataHolder = ameisenDataHolder;
            AmeisenDBManager = ameisenDBManager;
            AmeisenMovementEngine = ameisenMovementEngine;
            PartyPosition = 0;
        }

        public int PartyPosition { get; private set; }
        public override Start StartAction { get { return Start; } }
        public override DoThings StartDoThings { get { return DoThings; } }
        public override Exit StartExit { get { return Stop; } }

        public override void DoThings()
        {
            if (WaypointQueue.Count > 0)
            {
                // Do the movement stuff
                base.DoThings();
                Thread.Sleep(100);
                return;
            }

            RefreshActiveUnit();
            Me?.Update();
            ActiveUnit?.Update();

            if (Me == null || ActiveUnit == null)
            {
                return;
            }

            /*Vector3 posToMoveTo = ActiveUnit.pos;
            posToMoveTo = CalculateMovementOffset(
                posToMoveTo,
                GetFollowAngle(
                    GetPartymemberCount(),
                    GetMyPartyPosition()),
                AmeisenDataHolder.Settings.followDistance);*/

            //AmeisenMovementEngine.MemberCount = GetPartymemberCount();

            Vector4 targetPos = AmeisenMovementEngine.GetPosition(
                                    new Vector4(ActiveUnit.pos.X, ActiveUnit.pos.Y, ActiveUnit.pos.Z, ActiveUnit.Rotation),
                                    AmeisenDataHolder.Settings.followDistance / 10,
                                    GetMyPartyPosition());

            Vector3 posToMoveTo = new Vector3(targetPos.X, targetPos.Y, targetPos.Z);

            // When we are far enough away, follow
            if (WaypointQueue.Count == 0)
            {
                if (Utils.GetDistance(Me.pos, ActiveUnit.pos) > AmeisenDataHolder.Settings.followDistance)
                {
                    if (!WaypointQueue.Contains(posToMoveTo))
                    {
                        WaypointQueue.Enqueue(posToMoveTo);
                    }
                }
            }
        }

        /// <summary>
        /// Return a Player by the given GUID
        /// </summary>
        /// <param name="guid">guid of the player you want to get</param>
        /// <returns>Player that you want to get</returns>
        public WowObject GetWoWObjectFromGUID(ulong guid)
        {
            foreach (WowObject p in AmeisenDataHolder.ActiveWoWObjects)
            {
                if (p.Guid == guid)
                {
                    return p;
                }
            }

            return null;
        }

        public override void Start()
        {
            base.Start();

            Random rnd = new Random();
            XOffset = rnd.NextDouble() * AmeisenDataHolder.Settings.followDistance;
            YOffset = rnd.NextDouble() * AmeisenDataHolder.Settings.followDistance;
        }

        public override void Stop()
        {
            base.Stop();
        }

        private Unit ActiveUnit { get; set; }
        private AmeisenDataHolder AmeisenDataHolder { get; set; }
        private AmeisenDBManager AmeisenDBManager { get; set; }
        private AmeisenMovementEngine AmeisenMovementEngine { get; set; }

        private Me Me
        {
            get { return AmeisenDataHolder.Me; }
            set { AmeisenDataHolder.Me = value; }
        }

        private double XOffset { get; set; }
        private double YOffset { get; set; }

        private Vector3 CalculateMovementOffset(Vector3 posToMoveTo, double angle, double distance)
        {
            return new Vector3(
                posToMoveTo.X + (Math.Cos(angle) * (distance / 2) - XOffset),
                posToMoveTo.Y + (Math.Sin(angle) * (distance / 2) - YOffset),
                posToMoveTo.Z);
        }

        private double GetFollowAngle(int memberCount, int myPosition)
            => 2 * Math.PI / (memberCount * myPosition);

        private int GetMyPartyPosition()
        {
            Random rnd = new Random();

            if (PartyPosition != 0)
            {
                return PartyPosition;
            }

            if (AmeisenDataHolder.ActiveNetworkBots != null)
            {
                foreach (NetworkBot bot in AmeisenDataHolder.ActiveNetworkBots)
                {
                    PartyPosition++;
                    if (bot.GetSendableMe().Guid == Me.Guid)
                    {
                        break;
                    }
                }
            }
            else
            {
                if (PartyPosition == 0)
                {
                    PartyPosition = rnd.Next(1, 6);
                }
            }

            return PartyPosition;
        }

        private int GetPartymemberCount()
        {
            int count = 0; // subtract ourself
            foreach (ulong guid in Me.PartymemberGuids)
            {
                if (guid != 0)
                {
                    count++;
                }
            }

            return count;
        }

        private void RefreshActiveUnit()
        {
            foreach (Unit u in AmeisenDataHolder.Partymembers)
            {
                if (u != null)
                {
                    if (u.Guid != Me.PartyleaderGuid)
                    {
                        continue;
                    }

                    u.Update();
                    ActiveUnit = u;
                    return;
                }
            }

            ActiveUnit = (Unit)GetWoWObjectFromGUID(Me.PartyleaderGuid);
            AmeisenDataHolder.Partymembers.Add(ActiveUnit);
        }
    }
}