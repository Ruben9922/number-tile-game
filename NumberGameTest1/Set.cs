using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumberGameTest1
{
    class Set
    {
        public List<Tile> Tiles { get; set; } = new List<Tile>();
        public bool? IsRun { get; set; }

        public Set(List<Tile> tiles, bool isRun)
        {
            this.Tiles = tiles;
            this.IsRun = isRun;
        }

        public Set(List<Tile> tiles)
        {
            this.Tiles = tiles;
            this.IsRun = null;
        }
    }
}
