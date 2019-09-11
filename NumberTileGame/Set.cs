using System.Collections.Generic;
using System.Text;

namespace NumberTileGame
{
    internal class Set
    {
        public LinkedList<Tile> tiles = new LinkedList<Tile>();

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            int i = 0;
            foreach (Tile tile in tiles)
            {
                stringBuilder.Append(tile);
                
                if (i != tiles.Count - 1)
                {
                    stringBuilder.Append(", ");
                }

                stringBuilder.AppendLine();
            }

            return stringBuilder.ToString();
        }
    }
}