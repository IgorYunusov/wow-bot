namespace AmeisenBotUtilities.Structs
{
    public struct PathRequest
    {
        public PathRequest(Vector3 a, Vector3 b, int mapId)
        {
            A = a;
            B = b;
            MapId = mapId;
        }

        public Vector3 A { get; set; }
        public Vector3 B { get; set; }
        public int MapId { get; set; }
    }
}