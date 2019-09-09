using System;
using System.Collections.Generic;
using System.Linq;

namespace NumberTileGame
{
    internal class Game
    {
        private const int PlayerCount = 2;

        private IList<Player> players = new List<Player>(PlayerCount);

        internal void InputPlayerNames()
        {
            for (int i = 0; i < PlayerCount; i++)
            {
                string playerName;
                bool distinct;
                do
                {
                    Console.Write($"Name of Player {i + 1} [Player {i + 1}]: ");
                    playerName = Console.ReadLine();
                    if (playerName == "")
                    {
                        playerName = $"Player {i + 1}";
                    }
                    
                    distinct = CheckPlayerNameDistinctness(playerName);
                    if (!distinct)
                    {
                        Console.WriteLine("Player name already exists (note that this is case-insensitive)");
                    }
                } while (!distinct);

                players.Add(new Player(playerName));
                Console.WriteLine($"{playerName} added");
            }
        }

        private bool CheckPlayerNameDistinctness(string playerName)
        {
            IList<string> playerNames = players.Select(player => player.Name.ToLower()).ToList();
            playerNames.Add(playerName.ToLower());
            Console.WriteLine(playerNames.Distinct().Count());
            return playerNames.Distinct().Count() == playerNames.Count;
        }
    }
}