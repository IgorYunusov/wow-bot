using AmeisenBot.Character.Objects;
using AmeisenBot.Clients;
using AmeisenBotCombat;
using AmeisenBotCombat.Interfaces;
using AmeisenBotCore;
using AmeisenBotData;
using AmeisenBotPersistence;
using AmeisenBotUtilities;
using System.Collections.Generic;
using System.Threading;

namespace AmeisenBotFSM.Actions
{
    internal class ActionCombat : ActionMoving
    {
        public ActionCombat(
            AmeisenDataHolder ameisenDataHolder,
            IAmeisenCombatPackage combatPackage,
            AmeisenDBManager ameisenDBManager,
            AmeisenNavmeshClient ameisenNavmeshClient) : base(ameisenDataHolder, ameisenDBManager, ameisenNavmeshClient)
        {
            AmeisenDataHolder = ameisenDataHolder;
            CombatPackage = combatPackage;
        }

        public override void DoThings()
        {
            // Updte me, target and pet
            Me?.Update();
            Target?.Update();
            Pet?.Update();

            // Handle pending movement actions
            if (WaypointQueue.Count > 0)
            {
                base.DoThings();
            }

            // Try to get a target
            if (AmeisenDataHolder.IsHealer)
            {
                CombatUtils.TargetTargetToHeal(Me, AmeisenDataHolder.ActiveWoWObjects);
                Me?.Update();
                Target?.Update();
            }
            else
            {
                // clear all friendly targets
                AmeisenCore.ClearTargetIfItIsFriendly();

                if (Me.TargetGuid == 0 || CombatUtils.IsUnitValid(Target))
                {
                    CombatUtils.AssistParty(Me, AmeisenDataHolder.ActiveWoWObjects, AmeisenDataHolder.Partymembers);
                    // clear all friendly targets again
                    AmeisenCore.ClearTargetIfItIsFriendly();
                    Me?.Update();
                    Target?.Update();
                }

                if (Me.TargetGuid == 0 || CombatUtils.IsUnitValid(Target))
                {
                    CombatUtils.TargetNearestEnemy();
                    Me?.Update();
                    Target?.Update();

                    if (Me.TargetGuid == 0 || CombatUtils.IsUnitValid(Target))
                    {
                        // by now we should have a target
                        return;
                    }
                }
            }

            // Attack target if we are no healer
            if (!Me.InCombat && !AmeisenDataHolder.IsHealer) { CombatUtils.AttackTarget(); }

            // Cast the Spell selected for this Iteration
            if (CombatPackage.SpellStrategy != null)
            {
                Me?.Update();
                Target?.Update();

                Spell spellToUse = CombatPackage.SpellStrategy.DoRoutine(Me, Target, Pet);
                if (spellToUse != null)
                {
                    // try to get in line of sight
                    while (!IsInLineOfSight(Me, Target))
                    {
                        HandleMovement(Target.pos);
                        Thread.Sleep(500);
                    }

                    CombatUtils.CastSpellByName(Me, Target, spellToUse.Name, false, true);
                }
                else
                {
                    Thread.Sleep(200);
                }
            }

            // Handle Movement stuff
            if (CombatPackage.MovementStrategy != null)
            {
                Me?.Update();
                Target?.Update();
                HandleMovement(CombatPackage.MovementStrategy.CalculatePosition(Me, Target));
            }
        }

        public override void Start()
        {
            // Startup Method
            if (CombatPackage.SpellStrategy != null)
            {
                // Updte me, target and pet
                Me?.Update();
                Target?.Update();
                Pet?.Update();

                CombatPackage.SpellStrategy.Startup(Me, Target, Pet);
            }
            WaypointQueue.Clear();
            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
            WaypointQueue.Clear();
            AmeisenCore.RunSlashCommand("/cleartarget");
            Target = null;
        }

        private AmeisenDataHolder AmeisenDataHolder { get; set; }
        private IAmeisenCombatPackage CombatPackage { get; set; }

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

        private void HandleMovement(Vector3 pos)
        {
            Me.Update();

            if (!WaypointQueue.Contains(pos))
            {
                WaypointQueue.Enqueue(pos);
            }
        }

        private bool IsInLineOfSight(Me me, Unit target)
        {
            if (target == null)
            {
                return true;
            }

            List<Vector3> path = UsePathfinding(me.pos, target.pos);

            if (path == null)
            {
                return false;
            }

            if (path.Count == 1)
            {
                return true;
            }

            return false;
        }
    }
}