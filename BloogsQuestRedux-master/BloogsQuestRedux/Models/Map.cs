using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace BloogsQuestRedux.Models
{
    public class Map
    {
        readonly int Width;
        readonly int Height;
        public readonly int TileSize;

        public List<Tile> Tiles { get; set; }

        public Map(int width, int height, int tileSize, List<TilePrototype> tilePrototypes)
        {
            this.Width = width;
            this.Height = height;
            this.TileSize = tileSize;
            this.Tiles = new List<Tile>();

            var random = new Random();

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var tile = new Tile()
                    {
                        Position = new Vector2(x * TileSize, y * TileSize)
                    };
                    var rand = random.NextDouble();

                    if (rand > 0 && rand <= 0.84)
                    {
                        tile.Prototype = tilePrototypes.Single(t => t.TextureFilename == "dirt");
                    }
                    if (rand > 0.84 && rand <= 0.88)
                    {
                        tile.Prototype = tilePrototypes.Single(t => t.TextureFilename == "rock1");
                    }
                    if (rand > 0.88 && rand <= 0.92)
                    {
                        tile.Prototype = tilePrototypes.Single(t => t.TextureFilename == "rock2");
                    }
                    if (rand > 0.92 && rand <= 0.96)
                    {
                        tile.Prototype = tilePrototypes.Single(t => t.TextureFilename == "stump");
                    }
                    if (rand > 0.96)
                    {
                        tile.Prototype = tilePrototypes.Single(t => t.TextureFilename == "shrub");
                    }

                    this.Tiles.Add(tile);
                }
            }
        }

        public IEnumerable<Tile> GetBlockedTiles()
        {
            return Tiles.Where(x => !x.Prototype.IsPassable);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition)
        {
            foreach (var tile in Tiles)
            {
                // TODO: clean this up?
                if ((tile.Position.X >= cameraPosition.X - TileSize && tile.Position.X <= cameraPosition.X + Global.WindowWidth + TileSize) && (tile.Position.Y >= cameraPosition.Y - TileSize && tile.Position.Y <= cameraPosition.Y + Global.WindowHeight + TileSize))
                    spriteBatch.Draw(tile.Prototype.Sprite.Texture, new Vector2(tile.Position.X, tile.Position.Y), Color.White);
            }
        }
    }
}