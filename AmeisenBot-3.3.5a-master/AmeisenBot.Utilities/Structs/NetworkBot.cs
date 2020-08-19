using Newtonsoft.Json;

namespace AmeisenBotUtilities
{
    /// <summary>
    /// A Bot received from the server
    /// </summary>
    public struct NetworkBot
    {
        public int id;
        public string ip;
        public long lastActive;
        public string me;
        public string name;
        public string picture;

        public SendableMe GetSendableMe() => JsonConvert.DeserializeObject<SendableMe>(me);
    }
}