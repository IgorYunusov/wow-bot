﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using GreyMagic.Internals;
using IceFlake.Client.Patchables;
using IceFlake.DirectX;
using IceFlake.Runtime;

namespace IceFlake.Client
{
    public class WoWEvents : IPulsable
    {
        public WoWEvents()
        {
            var eventVictim = Manager.Memory.RegisterDelegate<LuaFunctionDelegate>(
                (IntPtr) Pointers.Events.EventVictim);
            if (_eventDetour == null || !_eventDetour.IsApplied)
                _eventDetour = Manager.Memory.Detours.CreateAndApply(eventVictim,
                    new LuaFunctionDelegate(HandleVictimCall),
                    "EventVictim");
        }

        private bool ListenerExists
        {
            get
            {
                const string checkCommand = "evcFrame == nil";
                List<string> ret = WoWScript.Execute(checkCommand);
                return ret[0] == "false";
            }
        }

        public void Direct3D_EndScene()
        {
            if ((DateTime.Now - _lastRegisterCheck).TotalMilliseconds >= RegisterCheckWait)
            {
                _lastRegisterCheck = DateTime.Now;
                if (!ListenerExists)
                    ExecuteIngameListener();
            }
        }

        public void Register(string name, EventHandler handler)
        {
            if (_eventHandler.ContainsKey(name))
                _eventHandler[name].Add(handler);
            else
                _eventHandler.Add(name, new List<EventHandler> {handler});
        }

        public void Remove(string name, EventHandler handler)
        {
            if (_eventHandler.ContainsKey(name))
                _eventHandler[name].Remove(handler);
        }

        private void HandleEvent(List<string> args)
        {
            string eventName = args[0];
            args.RemoveAt(0);
            if (_eventHandler.ContainsKey(eventName)) // First check for absolute matches
            {
                foreach (EventHandler handler in _eventHandler[eventName])
                    handler(eventName, args);
            }
            else // Then check for wildcard matches
            {
                foreach (var kv in _eventHandler)
                    if (eventName.WildcardMatch(kv.Key))
                        foreach (EventHandler handler in kv.Value)
                            handler(eventName, args);
            }
        }

        private int HandleVictimCall(IntPtr luaState)
        {
            int top = WoWScript.LuaInterface.GetTop(luaState);
            if (top > 0)
            {
                var args = new List<string>(top);
                for (int i = 1; i <= top; i++)
                    args.Add(WoWScript.LuaInterface.StackObjectToString(luaState, i));
                WoWScript.LuaInterface.Pop(luaState, top);
                HandleEvent(args);
            }
            else
            {
                return (int) _eventDetour.CallOriginal(luaState);
            }

            return 0;
        }

        private void ExecuteIngameListener()
        {
            const string command =
                "local frame = CreateFrame('Frame', 'evcFrame'); frame:RegisterAllEvents(); frame:SetScript('OnEvent', function(self, event, ...) GetBillingTimeRested(event, ...); end);";
            WoWScript.ExecuteNoResults(command);
        }

        #region Nested type: LuaFunctionDelegate

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int LuaFunctionDelegate(IntPtr luaState);

        #endregion

        #region Fields

        #region Delegates

        public delegate void EventHandler(string eventName, List<string> args);

        #endregion

        private const int RegisterCheckWait = 500; /*ms*/

        private static Detour _eventDetour;

        private readonly Dictionary<string, List<EventHandler>> _eventHandler =
            new Dictionary<string, List<EventHandler>>();

        private DateTime _lastRegisterCheck = DateTime.Now - TimeSpan.FromMilliseconds(RegisterCheckWait);

        #endregion
    }
}