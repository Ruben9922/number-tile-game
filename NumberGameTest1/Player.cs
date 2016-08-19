using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumberGameTest1
{
    class Player
    {
        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    name = value;
                }
            }
        }
        public List<Tile> Tiles { get; set; } = new List<Tile>();

        public Player(string name)
        {
            this.Name = name;
        }
    }
}
