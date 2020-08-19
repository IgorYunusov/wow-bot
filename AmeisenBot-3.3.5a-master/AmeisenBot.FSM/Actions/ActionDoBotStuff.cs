using AmeisenBot.Character;
using AmeisenBot.Character.Enums;
using AmeisenBot.Character.Objects;
using AmeisenBot.Clients;
using AmeisenBotData;
using AmeisenBotPersistence;
using AmeisenBotFSM.BotStuff;
using AmeisenBotFSM.Interfaces;
using System.Threading;
using static AmeisenBotFSM.Objects.Delegates;

namespace AmeisenBotFSM.Actions
{
    internal class ActionDoBotStuff : IAction
    {
        public ActionDoBotStuff(
            AmeisenDataHolder ameisenDataHolder,
            AmeisenDBManager ameisenDBManager,
            AmeisenCharacterManager ameisenCharacterManager,
            AmeisenNavmeshClient ameisenNavmeshClient)
        {
            AmeisenDataHolder = ameisenDataHolder;
            AmeisenDBManager = ameisenDBManager;
            AmeisenCharacterManager = ameisenCharacterManager;
            AmeisenNavmeshClient = ameisenNavmeshClient;
        }

        public Start StartAction { get { return Start; } }
        public DoThings StartDoThings { get { return DoThings; } }
        public Exit StartExit { get { return Stop; } }

        public void DoThings()
        {
            if (ThingTodo != null)
            {
                DoBotStuff(ThingTodo);
            }
            else
            {
                // got nothing to do
                Thread.Sleep(1000);
            }
        }

        public void Start()
        {
            ThingTodo = DecideWhatToDo();
        }

        public void Stop()
        {
        }

        private AmeisenCharacterManager AmeisenCharacterManager { get; set; }
        private AmeisenDataHolder AmeisenDataHolder { get; set; }
        private AmeisenDBManager AmeisenDBManager { get; set; }
        private AmeisenNavmeshClient AmeisenNavmeshClient { get; set; }
        private IAction ThingTodo { get; set; }

        private IAction DecideWhatToDo()
        {
            if (INeedToRepairMyStuff())
            {
                return new BotStuffRepairEquip(AmeisenDataHolder, AmeisenDBManager, AmeisenCharacterManager, AmeisenNavmeshClient);
            }
            else if (INeedToCleanMyBags())
            {
                return new BotStuffCleanBags(AmeisenDataHolder, AmeisenDBManager, AmeisenCharacterManager, AmeisenNavmeshClient);
            }

            // got nothing to do
            return null;
        }

        private void DoBotStuff(IAction whatToDo)
        {
            whatToDo.StartAction?.Invoke();
        }

        private bool INeedToCleanMyBags()
        {
            int grayCount = 0;
            foreach (Item item in AmeisenCharacterManager.Character.InventoryItems)
            {
                if (item != null && item.Id != 0 && item.Quality == ItemQuality.POOR)
                {
                    grayCount++;
                }
            }

            if (grayCount > 8) // need to check for full bags|| (&&grayCount > 0))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool INeedToRepairMyStuff()
        {
            int count = 0;
            double gearHealth = 0;
            bool somethingIsBroken = false;

            foreach (Item item in AmeisenCharacterManager.Character.Equipment.AsList())
            {
                if (item != null && item.Id != 0 && item.DurabilityMax > 0)
                {
                    count++;
                    gearHealth += item.Durability;
                    if (gearHealth == 0)
                    {
                        somethingIsBroken = true;
                    }
                }
            }
            gearHealth /= count;

            if (somethingIsBroken || gearHealth < 0.4)
            {
                return true;
            }

            return false;
        }
    }
}