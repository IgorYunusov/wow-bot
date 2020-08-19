using AmeisenBot.Character;
using AmeisenBot.Clients;
using AmeisenBotCombat;
using AmeisenBotCombat.Interfaces;
using AmeisenBotCore;
using AmeisenBotData;
using AmeisenBotPersistence;
using AmeisenBotFSM.Enums;
using AmeisenBotLogger;
using AmeisenBotUtilities;
using AmeisenMovement;
using System.Threading;

namespace AmeisenBotFSM
{
    public class AmeisenStateMachineManager
    {
        public AmeisenStateMachineManager(
            AmeisenDataHolder ameisenDataHolder,
            AmeisenDBManager ameisenDBManager,
            AmeisenMovementEngine ameisenMovementEngine,
            IAmeisenCombatPackage combatPackage,
            AmeisenCharacterManager characterManager,
            AmeisenNavmeshClient ameisenNavmeshClient)
        {
            Active = false;

            AmeisenDataHolder = ameisenDataHolder;
            AmeisenDBManager = ameisenDBManager;
            CombatPackage = combatPackage;
            AmeisenNavmeshClient = ameisenNavmeshClient;
            AmeisenCharacterManager = characterManager;
            AmeisenMovementEngine = ameisenMovementEngine;

            MainWorker = new Thread(new ThreadStart(DoWork));
            StateWatcherWorker = new Thread(new ThreadStart(WatchForStateChanges));
            StateMachine = new AmeisenStateMachine(ameisenDataHolder, ameisenDBManager, ameisenMovementEngine, combatPackage, characterManager, ameisenNavmeshClient);
        }

        public bool Active { get; private set; }
        public bool PushedCombat { get; private set; }
        public AmeisenStateMachine StateMachine { get; private set; }

        /// <summary>
        /// Fire up the FSM
        /// </summary>
        public void Start()
        {
            if (!Active)
            {
                Active = true;
                MainWorker.Start();
                StateWatcherWorker.Start();
            }
        }

        /// <summary>
        /// Shutdown the FSM
        /// </summary>
        public void Stop()
        {
            if (Active)
            {
                Active = false;
                MainWorker.Join();
                StateWatcherWorker.Join();
            }
        }

        public void UpdateCombatPackage(IAmeisenCombatPackage combatPackage)
        {
            CombatPackage = combatPackage;
            StateMachine = new AmeisenStateMachine(AmeisenDataHolder, AmeisenDBManager, AmeisenMovementEngine, combatPackage, AmeisenCharacterManager, AmeisenNavmeshClient);
        }

        private AmeisenCharacterManager AmeisenCharacterManager { get; set; }
        private AmeisenDataHolder AmeisenDataHolder { get; set; }
        private AmeisenDBManager AmeisenDBManager { get; set; }
        private AmeisenMovementEngine AmeisenMovementEngine { get; set; }
        private AmeisenNavmeshClient AmeisenNavmeshClient { get; set; }
        private IAmeisenCombatPackage CombatPackage { get; set; }
        private Thread MainWorker { get; set; }

        private Me Me
        {
            get { return AmeisenDataHolder.Me; }
            set { AmeisenDataHolder.Me = value; }
        }

        private Thread StateWatcherWorker { get; set; }

        private Unit Target
        {
            get { return AmeisenDataHolder.Target; }
            set { AmeisenDataHolder.Target = value; }
        }

        private bool BotStuffCheck()
        {
            if (AmeisenDataHolder.IsAllowedToDoOwnStuff)
            {
                StateMachine.PushAction(BotState.BotStuff);
                return true;
            }
            else
            {
                StateMachine.PopAction(BotState.BotStuff);
                return false;
            }
        }

        private bool DeadCheck()
        {
            if (AmeisenDataHolder.IsAllowedToRevive)
            {
                if (Me.IsDead) // || AmeisenCore.IsGhost(LuaUnit.player)
                {
                    StateMachine.PushAction(BotState.Dead);
                    return true;
                }
                else
                {
                    StateMachine.PopAction(BotState.Dead);
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Update the Statemachine, let it do its work
        /// </summary>
        private void DoWork()
        {
            while (Active)
            {
                // Do the Actions
                StateMachine.Update();
                Thread.Sleep(AmeisenDataHolder.Settings.stateMachineUpdateMillis);
            }
        }

        private bool FollowCheck()
        {
            if (Me.PartyleaderGuid != 0)
            {
                Unit activeUnit = null;
                foreach (WowObject p in AmeisenDataHolder.ActiveWoWObjects)
                {
                    if (p.Guid == Me.PartyleaderGuid)
                    {
                        activeUnit = (Unit)p;
                    }
                }

                Me.Update();
                activeUnit?.Update();

                if (activeUnit != null)
                {
                    double distance = Utils.GetDistance(Me.pos, activeUnit.pos);

                    if (AmeisenDataHolder.IsAllowedToFollowParty
                        && distance > AmeisenDataHolder.Settings.followDistance)
                    {
                        StateMachine.PushAction(BotState.Follow);
                        return true;
                    }
                    else if (StateMachine.GetCurrentState() == BotState.Follow)
                    {
                        StateMachine.PopAction(BotState.Follow);
                        return false;
                    }
                }
            }
            return false;
        }

        private bool InCombatCheck()
        {
            if (Me != null)
            {
                if (Me.InCombat
                    || (AmeisenDataHolder.IsAllowedToAssistParty
                    && CombatUtils.GetPartymembersInCombat(Me, AmeisenDataHolder.Partymembers).Count > 0))
                {
                    StateMachine.PushAction(BotState.Combat);
                    return true;
                }
                else
                {
                    StateMachine.PopAction(BotState.Combat);
                    return false;
                }
            }
            return false;
        }

        private bool IsLootThere()
        {
            if (AmeisenDataHolder.LootableUnits.Count > 0)
            {
                StateMachine.PushAction(BotState.Loot);
                return true;
            }
            else
            {
                StateMachine.PopAction(BotState.Loot);
                return false;
            }
        }

        private bool ReleaseSpiritCheck()
        {
            if (AmeisenDataHolder.IsAllowedToReleaseSpirit)
            {
                if (Me.Health == 0)
                {
                    AmeisenCore.ReleaseSpirit();
                    Thread.Sleep(1000);

                    while (AmeisenCore.IsInLoadingScreen())
                    {
                        Thread.Sleep(200);
                    }

                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Change the state of out FSM
        /// </summary>
        private void WatchForStateChanges()
        {
            while (Active)
            {
                Thread.Sleep(AmeisenDataHolder.Settings.stateMachineStateUpdateMillis);

                if (AmeisenCore.IsInLoadingScreen())
                {
                    continue;
                }

                // Am I in combat
                if (InCombatCheck())
                {
                    continue;
                }

                // Is there loot waiting for me
                if (IsLootThere())
                {
                    continue;
                }

                // Am I dead?
                if (DeadCheck())
                {
                    continue;
                }

                // Do I need to release my spirit
                if (ReleaseSpiritCheck())
                {
                    continue;
                }

                // Bot stuff check
                if (BotStuffCheck())
                {
                    continue;
                }

                // Is me supposed to follow
                if (FollowCheck())
                {
                    continue;
                }

                AmeisenLogger.Instance.Log(LogLevel.VERBOSE, $"FSM: {StateMachine.GetCurrentState()}", this);
            }
        }
    }
}