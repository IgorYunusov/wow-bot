using AmeisenBot.Clients;
using AmeisenBotCore;
using AmeisenBotData;
using AmeisenBotPersistence;
using AmeisenBotFSM.Interfaces;
using AmeisenBotLogger;
using AmeisenBotUtilities;
using AmeisenBotUtilities.Structs;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using static AmeisenBotFSM.Objects.Delegates;

namespace AmeisenBotFSM.Actions
{
    public class ActionMoving : IAction
    {
        public ActionMoving(AmeisenDataHolder ameisenDataHolder, AmeisenDBManager ameisenDBManager, AmeisenNavmeshClient ameisenNavmeshClient)
        {
            AmeisenDataHolder = ameisenDataHolder;
            AmeisenDBManager = ameisenDBManager;
            AmeisenNavmeshClient = ameisenNavmeshClient;

            WaypointQueue = new Queue<Vector3>();
            LastPosition = new Vector3(int.MaxValue, int.MaxValue, int.MaxValue);
        }

        public Vector3 LastEnqued { get; internal set; }
        public virtual Start StartAction { get { return Start; } }
        public virtual DoThings StartDoThings { get { return DoThings; } }
        public virtual Exit StartExit { get { return Stop; } }
        public Queue<Vector3> WaypointQueue { get; set; }

        public virtual void DoThings()
        {
            if (WaypointQueue.Count > 0)
            {
                /*double distance = Utils.GetDistance(Me.pos, WaypointQueue.Peek());

                if (AmeisenDataHolder.Settings.landMounts != ""
                    && distance > AmeisenDataHolder.Settings.useMountFollowDistance
                    && AmeisenCore.IsOutdoors
                    && !AmeisenCore.IsMounted)
                {
                    AmeisenCore.MountRandomMount(AmeisenDataHolder.Settings.landMounts, AmeisenDataHolder.Settings.flyingMounts);
                    Thread.Sleep(1500);
                }

                if (distance < AmeisenDataHolder.Settings.useMountFollowDistance + 6
                    && AmeisenCore.IsMounted)
                {
                    AmeisenCore.MountRandomMount(AmeisenDataHolder.Settings.landMounts, AmeisenDataHolder.Settings.flyingMounts);
                }*/

                MoveToNode();
            }
        }

        public virtual void Start()
        {
        }

        public virtual void Stop()
        {
        }

        public List<Vector3> UsePathfinding(Vector3 initialPosition, Vector3 targetPosition)
        {
            Me.Update();
            return AmeisenNavmeshClient.RequestPath(new PathRequest(initialPosition, targetPosition, Me.MapId));
        }

        private int movementDistance = 3;
        private AmeisenDataHolder AmeisenDataHolder { get; set; }
        private AmeisenDBManager AmeisenDBManager { get; set; }
        private AmeisenNavmeshClient AmeisenNavmeshClient { get; set; }
        private Vector3 LastPosition { get; set; }

        private Me Me
        {
            get { return AmeisenDataHolder.Me; }
            set { AmeisenDataHolder.Me = value; }
        }

        private double MovedSinceLastTick { get; set; }

        /// <summary>
        /// Very basic Obstacle avoidance.
        ///
        /// Need to change this to a better waypoint system that uses our MapNode Database...
        /// </summary>
        /// <param name="initialPosition">initial position</param>
        /// <param name="activePosition">position now</param>
        /// <returns>if we havent moved 0.5m in the 2 vectors, jump and return true</returns>
        private bool CheckIfWeAreStuckIfYesJump(Vector3 initialPosition, Vector3 activePosition)
        {
            MovedSinceLastTick = Utils.GetDistance(initialPosition, activePosition);

            // we are possibly stuck at a fence or something alike
            if (MovedSinceLastTick != 0 && MovedSinceLastTick < 1000)
            {
                if (MovedSinceLastTick < AmeisenDataHolder.Settings.movementJumpThreshold)
                {
                    AmeisenCore.CharacterJumpAsync();
                    //AmeisenCore.MoveLeftRight();
                    AmeisenLogger.Instance.Log(LogLevel.DEBUG, $"Jumping: {MovedSinceLastTick}", this);
                    return true;
                }
            }
            return false;
        }

        private void MoveToNode()
        {
            if (WaypointQueue.Count > 0)
            {
                Me.Update();
                Vector3 initialPosition = Me.pos;
                if (WaypointQueue.Count == 0)
                {
                    return;
                }

                Vector3 targetPosition = WaypointQueue.Dequeue();
                double distance = Utils.GetDistance(initialPosition, targetPosition);

                if (distance > AmeisenDataHolder.Settings.followDistance)
                {
                    CheckIfWeAreStuckIfYesJump(Me.pos, LastPosition);

                    if (targetPosition.Z == 0)
                    {
                        targetPosition.Z = Me.pos.Z;
                    }

                    List<Vector3> navmeshPath = new List<Vector3>() { targetPosition };

                    if (distance > AmeisenDataHolder.Settings.pathfindingUsageThreshold)
                    {
                        navmeshPath = UsePathfinding(Me.pos, targetPosition);

                        if (navmeshPath == null || navmeshPath.Count == 0)
                        {
                            Thread.Sleep(1000);
                            return;
                        }

                        if (navmeshPath.Count > 1)
                        {
                            navmeshPath.Add(targetPosition); // original position
                        }

                        movementDistance = AmeisenCore.IsMounted ? 8 : 3; // Mount distance adjustments

                        foreach (Vector3 pos in navmeshPath)
                        {
                            Me.Update();
                            double posDistance = Utils.GetDistance(Me.pos, pos);
                            int tries = 0;

                            if (posDistance < movementDistance)
                            {
                                continue;
                            }

                            while (tries < 10 && posDistance > movementDistance)
                            {
                                AmeisenCore.MovePlayerToXYZ(pos, InteractionType.MOVE);
                                posDistance = Utils.GetDistance(Me.pos, pos);
                                tries++;
                                Thread.Sleep(20);
                            }

                            Me.Update();
                            double distanceTraveled = posDistance - Utils.GetDistance(Me.pos, pos);
                            // if we havent moved 0.2m in this time, screw this path
                            if (tries == 10 && distanceTraveled < 0.2)
                            {
                                WaypointQueue.Clear();
                                break;
                            }
                        }
                    }
                    else
                    {
                        AmeisenCore.MovePlayerToXYZ(navmeshPath.First(), InteractionType.MOVE);
                    }

                    Me.Update();
                    LastPosition = Me.pos;
                }
            }
            else { }
        }
    }
}