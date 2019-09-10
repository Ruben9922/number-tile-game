using System.Collections;

namespace NumberTileGame
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Game game = new Game();
            game.InputPlayerNames();
            game.GenerateTiles();
        }
    }
}