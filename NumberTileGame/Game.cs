using System;
using System.Collections.Generic;
using System.Linq;

namespace NumberTileGame
{
    internal class Game
    {
        private const int PlayerCount = 2;

        private IList<Player> players = new List<Player>(PlayerCount);
        private IList<Tile> tiles = new List<Tile>(((NumberTile.MaxValue - NumberTile.MinValue + 1) * 8) + 2);

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

        internal void GenerateTiles()
        {
            // Generate number tiles
            for (int i = NumberTile.MinValue; i <= NumberTile.MaxValue; i++) // For each number
            {
                foreach (Colour colour in (Colour[]) Enum.GetValues(typeof(Colour))) // For each colour
                {
                    for (int j = 0; j < 2; j++) // For each of two copies
                    {
                        tiles.Add(new NumberTile(colour, i));
                    }
                }
            }
            
            // Generate smiley tiles
            tiles.Add(new SmileyTile(Colour.Black));
            tiles.Add(new SmileyTile(Colour.Orange));

            foreach (Tile tile in tiles)
            {
                Console.WriteLine(tile);
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