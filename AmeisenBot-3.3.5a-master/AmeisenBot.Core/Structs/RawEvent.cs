using System.Collections.Generic;

namespace AmeisenBotCore.Structs
{
    public struct RawEvent
    {
        public string @event;
        public List<string> args;
        public long time;
    }
}