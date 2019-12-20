using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BloogsQuestRedux.Models
{
    public class Sprite
    {
        public Texture2D Texture { get; set; }

        public void LoadContent(ContentManager content, string assetName)
        {
            Texture = content.Load<Texture2D>(assetName);
        }
    }
}