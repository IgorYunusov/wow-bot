using AmeisenBotCore.Structs;
using AmeisenBotLogger;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace AmeisenBotCore
{
    public class AmeisenEventHook
    {
        public AmeisenEventHook()
        {
            EventReader = new Thread(new ThreadStart(ReadEvents));
            EventDictionary = new Dictionary<string, OnEventFired>();
        }

        public delegate void OnEventFired(long timestamp, List<string> args);

        public bool IsActive { get; private set; }

        /// <summary>
        /// Start to receive events to our event table
        /// and start the event reader that will fire 
        /// events if they apper in our event table.
        /// 
        /// Events will get read every 1000 ms by now.
        /// </summary>
        public void Init()
        {
            StringBuilder luaStuff = new StringBuilder();
            luaStuff.Append("abFrame = CreateFrame(\"FRAME\", \"AbotEventFrame\") ");
            luaStuff.Append("abEventTable = {} ");
            luaStuff.Append("function abEventHandler(self, event, ...) ");
            luaStuff.Append("table.insert(abEventTable, {time(), event, {...}}) end ");
            luaStuff.Append("if abFrame:GetScript(\"OnEvent\") == nil then ");
            luaStuff.Append("abFrame:SetScript(\"OnEvent\", abEventHandler) end");
            AmeisenCore.LuaDoString(luaStuff.ToString());
            
            IsActive = true;

            EventReader.Start();
            // if we equip an item confirm the dialog
            AmeisenCore.EnableAutoBoPConfirm();
        }

        public void Stop()
        {
            if (IsActive)
            {
                AmeisenCore.LuaDoString($"abFrame:UnregisterAllEvents();");
                AmeisenCore.LuaDoString("abFrame:SetScript(\"OnEvent\", nil)");
                IsActive = false;
                EventReader.Join();
            }
        }

        /// <summary>
        /// Subscribe to an event
        /// </summary>
        /// <param name="eventName">event name</param>
        /// <param name="onEventFired">method to fire when the event appered in WoW</param>
        public void Subscribe(string eventName, OnEventFired onEventFired)
        {
            AmeisenCore.LuaDoString($"abFrame:RegisterEvent(\"{eventName}\");");
            EventDictionary.Add(eventName, onEventFired);
        }

        /// <summary>
        /// Unsubscribe from an event
        /// </summary>
        /// <param name="eventName">event name</param>
        public void Unsubscribe(string eventName)
        {
            AmeisenCore.LuaDoString($"abFrame:UnregisterEvent(\"{eventName}\");");
            EventDictionary.Remove(eventName);
        }

        public Dictionary<string, OnEventFired> EventDictionary { get; private set; }
        private Thread EventReader { get; set; }

        private void ReadEvents()
        {
            while (IsActive)
            {
                if (AmeisenCore.IsInLoadingScreen())
                {
                    Thread.Sleep(50);
                    continue;
                }

                // Unminified lua code can be found im my github repo "WowLuaStuff"
                string eventJson = AmeisenCore.GetLocalizedText("abEventJson='['for a,b in pairs(abEventTable)do abEventJson=abEventJson..'{'for c,d in pairs(b)do if type(d)==\"table\"then abEventJson=abEventJson..'\"args\": ['for e,f in pairs(d)do abEventJson=abEventJson..'\"'..f..'\"'if e<=table.getn(d)then abEventJson=abEventJson..','end end;abEventJson=abEventJson..']}'if a<table.getn(abEventTable)then abEventJson=abEventJson..','end else if type(d)==\"string\"then abEventJson=abEventJson..'\"event\": \"'..d..'\",'else abEventJson=abEventJson..'\"time\": \"'..d..'\",'end end end end;abEventJson=abEventJson..']'abEventTable={}", "abEventJson");
                AmeisenLogger.Instance.Log(LogLevel.VERBOSE, $"LUA Events Json: {eventJson}", this);

                List<RawEvent> rawEvents = new List<RawEvent>();
                try
                {
                    // parse the events from JSON
                    List<RawEvent> finalEvents = new List<RawEvent>();
                    rawEvents = JsonConvert.DeserializeObject<List<RawEvent>>(eventJson);

                    foreach (RawEvent rawEvent in rawEvents)
                    {
                        if (!finalEvents.Contains(rawEvent))
                        {
                            finalEvents.Add(rawEvent);
                        }
                    }

                    // Fire the events
                    AmeisenLogger.Instance.Log(LogLevel.VERBOSE, $"Parsed {finalEvents.Count} events", this);
                    if (finalEvents.Count > 0)
                    {
                        foreach (RawEvent rawEvent in finalEvents)
                        {
                            if (EventDictionary.ContainsKey(rawEvent.@event))
                            {
                                EventDictionary[rawEvent.@event].Invoke(rawEvent.time, rawEvent.args);
                                AmeisenLogger.Instance.Log(LogLevel.VERBOSE, $"Fired OnEventFired: {rawEvent.@event}", this);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    AmeisenLogger.Instance.Log(LogLevel.ERROR, $"Failed to parse events Json: {e}", this);
                }

                Thread.Sleep(1000);
            }
        }
    }
}