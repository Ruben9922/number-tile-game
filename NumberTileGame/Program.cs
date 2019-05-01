using System;

namespace NumberTileGame
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            const int playerCount = 2;
            Player[] players = new Player[playerCount];

            for (int i = 0; i < playerCount; i++)
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

    internal class Player
    {
        private string PlayerName { get; set; }

        public Player(string playerName)
        {
            PlayerName = playerName;
        }
    }
}