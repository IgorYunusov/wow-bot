using System.Collections.Generic;

namespace BloogsQuestRedux.Models
{
    public class Scene
    {
        public Player Player { get; private set; }
        public Map Map { get; private set; }
        public Camera Camera { get; private set; }
        public List<Trigger> Triggers { get; private set; }

        public Scene(Map map, Player player, Camera camera)
        {
            this.Map = map;
            this.Player = player;
            this.Camera = camera;
        }
    }
}