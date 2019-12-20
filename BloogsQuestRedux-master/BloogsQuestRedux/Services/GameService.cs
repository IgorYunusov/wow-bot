using System.Collections.Generic;

using BloogsQuestRedux.Models;

namespace BloogsQuestRedux.Services
{
    public class GameService
    {
        public List<TilePrototype> GetTilePrototypes()
        {
            var tilePrototypes = new List<TilePrototype>();

            tilePrototypes.Add(new TilePrototype()
            {
                IsPassable = true,
                TextureFilename = "dirt",
                Sprite = new Sprite()
            });

            tilePrototypes.Add(new TilePrototype()
            {
                IsPassable = true,
                TextureFilename = "darkdirt",
                Sprite = new Sprite()
            });

            tilePrototypes.Add(new TilePrototype()
            {
                IsPassable = true,
                TextureFilename = "wetdirt",
                Sprite = new Sprite()
            });

            tilePrototypes.Add(new TilePrototype()
            {
                IsPassable = false,
                TextureFilename = "rock1",
                Sprite = new Sprite()
            });

            tilePrototypes.Add(new TilePrototype()
            {
                IsPassable = false,
                TextureFilename = "rock2",
                Sprite = new Sprite()
            });

            tilePrototypes.Add(new TilePrototype()
            {
                IsPassable = false,
                TextureFilename = "shrub",
                Sprite = new Sprite()
            });

            tilePrototypes.Add(new TilePrototype()
            {
                IsPassable = false,
                TextureFilename = "stump",
                Sprite = new Sprite()
            });

            return tilePrototypes;
        }
    }
}