using AmeisenBotCombat;
using AmeisenBotCore;
using AmeisenBotData;
using AmeisenBotPersistence;
using AmeisenBotUtilities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;

namespace AmeisenBotManager
{
    public class AmeisenObjectManager : IDisposable
    {
        public AmeisenObjectManager(AmeisenDataHolder ameisenDataHolder, AmeisenDBManager ameisenDBManager)
        {
            AmeisenDataHolder = ameisenDataHolder;
            AmeisenDBManager = ameisenDBManager;
            RefreshObjects();
            AmeisenDataHolder.Partymembers = CombatUtils.GetPartymembers(Me, ActiveWoWObjects);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Return a Player by the given GUID
        /// </summary>
        /// <param name="guid">guid of the player you want to get</param>
        /// <returns>Player that you want to get</returns>
        public WowObject GetWoWObjectFromGUID(ulong guid)
        {
            foreach (WowObject p in ActiveWoWObjects)
            {
                if (p.Guid == guid)
                {
                    return p;
                }
            }

            return null;
        }

        /// <summary>
        /// Starts the ObjectUpdates
        /// </summary>
        public void Start()
        {
            RefreshObjects();

            // Timer to update the objects from memory
            objectUpdateTimer = new System.Timers.Timer(AmeisenDataHolder.Settings.dataRefreshRate);
            objectUpdateTimer.Elapsed += ObjectUpdateTimer;
            objectUpdateThread = new Thread(new ThreadStart(StartTimer));
            objectUpdateThread.Start();
        }

        /// <summary>
        /// Stops the ObjectUpdates
        /// </summary>
        public void Stop()
        {
            objectUpdateTimer.Stop();
            objectUpdateThread.Join();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                ((IDisposable)objectUpdateTimer).Dispose();
            }
        }

        private Thread objectUpdateThread;
        private System.Timers.Timer objectUpdateTimer;

        private List<WowObject> ActiveWoWObjects
        {
            get { return AmeisenDataHolder.ActiveWoWObjects; }
            set { AmeisenDataHolder.ActiveWoWObjects = value; }
        }

        private AmeisenDataHolder AmeisenDataHolder { get; set; }
        private AmeisenDBManager AmeisenDBManager { get; set; }

        private Me Me
        {
            get { return AmeisenDataHolder.Me; }
            set { AmeisenDataHolder.Me = value; }
        }

        private Unit Pet
        {
            get { return AmeisenDataHolder.Pet; }
            set { AmeisenDataHolder.Pet = value; }
        }

        private Unit Target
        {
            get { return AmeisenDataHolder.Target; }
            set { AmeisenDataHolder.Target = value; }
        }

        private void AntiAFK() => AmeisenCore.AntiAFK();

        private void ObjectUpdateTimer(object source, ElapsedEventArgs e) => RefreshObjects();

        private void RefreshObjects()
        {
            if (AmeisenCore.IsInLoadingScreen()) { return; }

            ActiveWoWObjects = AmeisenCore.GetAllWoWObjects();

            foreach (WowObject t in ActiveWoWObjects)
            {
                // update objects that are new
                if (t == null && t.Guid == 0) { t.Update(); }

                // Get the Me object
                if (t.GetType() == typeof(Me))
                {
                    t.Update();
                    Me = (Me)t;
                    Me.ZoneId = AmeisenCore.GetZoneId();
                    Me.MapId = AmeisenCore.GetMapId();
                    continue;
                }

                // Get our Pet object
                if (Me != null && t.Guid == Me.PetGuid)
                {
                    t.Update();
                    t.Distance = Utils.GetDistance(Me.pos, t.pos);
                    Pet = (Unit)t;
                    continue;
                }

                // Get our Target object
                if (Me != null && t.Guid == Me.TargetGuid)
                {
                    t.Update();

                    if (t.GetType() == typeof(Me))
                    {
                        Target = (Me)t;
                        continue;
                    }
                    else if (t.GetType() == typeof(Player))
                    {
                        t.Distance = Utils.GetDistance(Me.pos, t.pos);
                        Target = (Player)t;
                        continue;
                    }
                    else if (t.GetType() == typeof(Unit))
                    {
                        t.Distance = Utils.GetDistance(Me.pos, t.pos);
                        Target = (Unit)t;
                        continue;
                    }
                }
            }

            // Get Partymembers
            // will be mainly handled by an event but should get refreshed once in a while
            // AmeisenDataHolder.Partymembers = CombatUtils.GetPartymembers(Me, ActiveWoWObjects);

            // Update Partymembers
            if (AmeisenDataHolder.Partymembers != null)
            {
                foreach (Unit unit in AmeisenDataHolder.Partymembers)
                {
                    if (unit != null)
                    {
                        unit.Update();
                    }
                }
            }

            // Best place for this :^)
            AntiAFK();
        }

        private void StartTimer() => objectUpdateTimer.Start();
    }
}