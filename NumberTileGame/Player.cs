using System.Collections.Generic;

namespace NumberTileGame
{
    internal class Player
    {
        public const int TileCount = 14;

        internal string Name { get; set; }
        public IList<Tile> Tiles { get; } = new List<Tile>(TileCount);

        public Player(string name)
        {
            Name = name;
        }
    }
}