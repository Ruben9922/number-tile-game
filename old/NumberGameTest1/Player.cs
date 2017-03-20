using System.Collections.Generic;

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
        public bool IsPlaying { get; set; } = true;

        public Player(string name)
        {
            Name = name;
        }
    }
}
