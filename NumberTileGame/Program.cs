using System;

namespace NumberTileGame
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Game game = new Game();
            game.Play();
        }
    }

    internal class Player
    {
        private string PlayerName { get; set; }

        public Player(string playerName)
        {
            PlayerName = playerName;
        }
    }

    internal class Game
    {
        private const int PlayerCount = 2;

        private Player[] players = new Player[PlayerCount];

        internal void Play()
        {
            for (int i = 0; i < PlayerCount; i++)
            {
                Console.Write($"Name of Player {i + 1} [Player {i + 1}]: ");
                string playerName = Console.ReadLine();
                if (playerName == "")
                {
                    playerName = $"Player {i + 1}";
                }

                players[i] = new Player(playerName);
            }
        }
    }
}