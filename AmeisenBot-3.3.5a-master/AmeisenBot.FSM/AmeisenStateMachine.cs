using AmeisenBot.Character;
using AmeisenBot.Clients;
using AmeisenBotCombat.Interfaces;
using AmeisenBotData;
using AmeisenBotPersistence;
using AmeisenBotFSM.Actions;
using AmeisenBotFSM.BotStuff;
using AmeisenBotFSM.Enums;
using AmeisenBotFSM.Interfaces;
using AmeisenBotLogger;
using AmeisenMovement;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AmeisenBotFSM
{
    /// <summary>
    /// This is a Stack based FSM that will manage our bots actions
    ///
    /// You can Push a BotState and Pop a BotState from the stack. This allows you to manage the bots
    /// executed logic. (sidenote: duplicate BotStates wont be added to the Stack)
    ///
    /// There is a Bot enum, wich an IAction gets mapped to using a Dictionary. This IAction has to
    /// implement 3 methods:
    ///
    /// Start:      will be called on first call only
    /// DoThings:   will be called everytime Update(); is called
    /// Exit:       will be called after the last call only
    ///
    /// In this 3 methods you can implement logic or so...
    ///
    /// And remember to call Update(); on a frequent basis, else this thing isnt going to do anything.
    /// </summary>
    public class AmeisenStateMachine
    {
        public AmeisenStateMachine(
            AmeisenDataHolder ameisenDataHolder,
            AmeisenDBManager ameisenDBManager,
            AmeisenMovementEngine ameisenMovementEngine,
            IAmeisenCombatPackage combatPackage,
            AmeisenCharacterManager ameisenCharacterManager,
            AmeisenNavmeshClient ameisenNavmeshClient)
        {
            StateStack = new Stack<BotState>();
            StateActionMap = new Dictionary<BotState, IAction>
            {
                { BotState.Idle, new ActionIdle(ameisenDataHolder) },
                { BotState.Follow, new ActionFollow(ameisenDataHolder, ameisenDBManager, ameisenMovementEngine, ameisenNavmeshClient) },
                { BotState.Moving, new ActionMoving(ameisenDataHolder, ameisenDBManager, ameisenNavmeshClient) },
                { BotState.Combat, new ActionCombat(ameisenDataHolder, combatPackage, ameisenDBManager, ameisenNavmeshClient) },
                { BotState.Dead, new ActionDead(ameisenDataHolder, ameisenDBManager, ameisenNavmeshClient) },
                { BotState.Loot, new ActionLoot(ameisenDataHolder, ameisenDBManager, ameisenNavmeshClient) },
                { BotState.BotStuff, new ActionDoBotStuff(ameisenDataHolder, ameisenDBManager, ameisenCharacterManager, ameisenNavmeshClient) }
            };

            BotStuffList = new List<IAction>() {
                new BotStuffRepairEquip(ameisenDataHolder, ameisenDBManager, ameisenCharacterManager,ameisenNavmeshClient)
            };
        }

        public List<IAction> BotStuffList { get; private set; }

        /// <summary>
        /// Returns our current BotState to see what the bot is doing right now
        /// </summary>
        /// <returns>current BotState</returns>
        public BotState GetCurrentState() => StateStack.Count > 0 ? StateStack.Peek() : BotState.None;

        public void LoadNewCombatClass(
            AmeisenDataHolder ameisenDataHolder,
            IAmeisenCombatPackage combatPackage,
            AmeisenDBManager ameisenDBManager,
            AmeisenNavmeshClient ameisenNavmeshClient)
            => StateActionMap[BotState.Combat] = new ActionCombat(ameisenDataHolder, combatPackage, ameisenDBManager, ameisenNavmeshClient);

        /// <summary>
        /// Pop the state Stack of the bot, calls the Start() of new State and the Stop() of current State.
        /// </summary>
        /// <param name="botState">the state you want to change to</param>
        public BotState PopAction(BotState botState, [CallerMemberName]string functionName = "")
        {
            if (GetCurrentState() == botState)
            {
                AmeisenLogger.Instance.Log(LogLevel.VERBOSE, $"FSM Pop called by: {functionName}", this);
                GetCurrentStateAction(GetCurrentState())?.StartExit.Invoke();
                BotState tmpState = StateStack.Pop();
                GetCurrentStateAction(GetCurrentState())?.StartAction.Invoke();
                return tmpState;
            }
            return GetCurrentState();
        }

        /// <summary>
        /// Push something onto the state Stack of the bot, this calls the Stop() of current State
        /// and the Start() of new State.
        /// </summary>
        /// <param name="botState">the state you want to change to</param>
        public void PushAction(BotState botState, [CallerMemberName]string functionName = "")
        {
            if (GetCurrentState() != botState)
            {
                AmeisenLogger.Instance.Log(LogLevel.VERBOSE, $"FSM Push [{botState}] called by: {functionName}", this);
                GetCurrentStateAction(GetCurrentState())?.StartExit.Invoke();
                StateStack.Push(botState);
                GetCurrentStateAction(GetCurrentState())?.StartAction.Invoke();
            }
        }

        /// <summary>
        /// Call this to Update the Statemachine and execute actions
        /// </summary>
        public void Update() => GetCurrentStateAction(GetCurrentState())?.StartDoThings.Invoke();

        private Dictionary<BotState, IAction> StateActionMap { get; set; }
        private Stack<BotState> StateStack { get; set; }

        /// <summary>
        /// Map the BotState to an IAction containing Start(), DoThings() and Stop()
        /// </summary>
        /// <param name="state">state to map</param>
        /// <returns>IAction</returns>
        private IAction GetCurrentStateAction(BotState state) => state == BotState.None ? null : StateActionMap[state];
    }
}