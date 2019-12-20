﻿using System;
using System.Collections.Generic;
using System.Linq;
using IceFlake.Client.Objects;
using IceFlake.DirectX;
using IceFlake.Runtime;

namespace IceFlake.Client.Routines
{
    public abstract class RoutineBrain : IPulsable
    {
        public Func<IEnumerable<WoWUnit>> HarmfulTargetsSelector;
        public Func<IEnumerable<WoWUnit>> HelpfulTargetsSelector;

        #region Default Target Selectors

        private Func<IEnumerable<WoWUnit>> DefaultHarmfulTargetsSelector
        {
            get
            {
                return () =>
                    from u in
                        Manager.ObjectManager.Objects.Where(o => o.IsValid && (o.IsUnit || o.IsPlayer))
                            .OfType<WoWUnit>()
                    where u.IsValid &&
                          u.Distance < 40 &&
                          !u.IsFriendly &&
                          !u.IsDead &&
                          u.IsInCombat
                    orderby u.Distance ascending
                    select u;
            }
        }

        private Func<IEnumerable<WoWUnit>> TankHarmfulTargetsSelector
        {
            get
            {
                return () =>
                    from u in
                        Manager.ObjectManager.Objects.Where(o => o.IsValid && (o.IsUnit || o.IsPlayer))
                            .OfType<WoWUnit>()
                    where u.IsValid &&
                          u.Distance < 40 &&
                          !u.IsFriendly &&
                          !u.IsDead &&
                          u.IsInCombat
                    orderby u.CalculateThreat ascending, u.HealthPercentage ascending, u.Distance ascending
                    select u;
            }
        }

        private Func<IEnumerable<WoWUnit>> DefaultHelpfulTargetsSelector
        {
            get
            {
                return () =>
                    from u in WoWParty.Members
                    where u.IsValid
                          && u.Distance < 40
                          && !u.IsDead
                          && u.IsFriendly
                    orderby u.HealthPercentage ascending
                    select u;
            }
        }

        private Func<IEnumerable<WoWUnit>> RaidHelpfulTargetsSelector
        {
            get
            {
                return () =>
                    Manager.ObjectManager.Objects
                        .Where(x => x.IsValid && x.IsPlayer)
                        .OfType<WoWPlayer>()
                        .Where(x => x.Distance < 40 && !x.IsDead && x.IsFriendly);
            }
        }

        #endregion

        public RoutineBrain()
        {
            Actions = new List<RoutineAction>();

            Manager.Events.Register("PLAYER_REGEN_DISABLED", HandleCombatEvents);
            Manager.Events.Register("PLAYER_REGEN_ENABLED", HandleCombatEvents);

            HarmfulTargetsSelector = DefaultHarmfulTargetsSelector;
            HelpfulTargetsSelector = DefaultHelpfulTargetsSelector;

            Direct3D.RegisterCallback(this);
        }

        public bool IsRunning { get; private set; }
        private List<RoutineAction> Actions { get; set; }
        private DateTime SleepTime { get; set; }
        public IEnumerable<WoWUnit> HarmfulTargets { get; private set; }

        public WoWUnit HarmfulTarget { get; private set; }

        public IEnumerable<WoWUnit> HelpfulTargets { get; private set; }

        public WoWUnit HelpfulTarget { get; private set; }

        public WoWUnit AlternativeHelpfulTarget { get; private set; }

        public void Direct3D_EndScene()
        {
            if (!IsRunning)
                return;

            if (!Manager.ObjectManager.IsInGame)
                return;

            if (Manager.LocalPlayer.IsDead)
                return;

            if (SleepTime >= DateTime.Now)
                return;

            SelectTargets();

            RoutineAction action = Actions
                .Where(a => a.IsWanted && a.IsReady)
                .OrderByDescending(a => a.Priority)
                .FirstOrDefault();

            try
            {
                OnBeforeAction(action);
                if (action != null)
                    action.Execute();
                OnAfterAction(action);
            }
            catch (SleepException ex)
            {
                SleepTime = DateTime.Now + ex.Time;
            }
            catch (Exception ex)
            {
                Log.WriteLine("Exception in Brain: {0}", ex.ToLongString());
            }
        }

        ~RoutineBrain()
        {
            Direct3D.RemoveCallback(this);
        }

        public void Start()
        {
            IsRunning = true;
        }

        public void Stop()
        {
            IsRunning = false;
        }

        protected void AddAction(RoutineAction action)
        {
            Actions.Add(action);
        }

        private void HandleCombatEvents(string ev, List<string> args)
        {
            switch (ev)
            {
                case "PLAYER_REGEN_DISABLED":
                    OnEnterCombat();
                    break;
                case "PLAYER_REGEN_ENABLED":
                    OnExitCombat();
                    break;
            }
        }

        public void SelectTargets()
        {
            HarmfulTargets = HarmfulTargetsSelector();
            HarmfulTarget = HarmfulTargets.FirstOrDefault();

            HelpfulTargets = HelpfulTargetsSelector();
            HelpfulTarget = HelpfulTargets.FirstOrDefault();
            AlternativeHelpfulTarget = HelpfulTargets.ElementAtOrDefault(1);
        }

        protected abstract void OnBeforeAction(RoutineAction action);
        protected abstract void OnAfterAction(RoutineAction action);

        public virtual void OnEnterCombat()
        {
        }

        public virtual void OnExitCombat()
        {
        }

        public void Sleep(int ms)
        {
            throw new SleepException(ms);
        }

        public void Print(string text, params object[] args)
        {
            Log.WriteLine(text, args);
        }
    }
}