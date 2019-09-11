using System;
using System.Collections.Generic;
using System.Linq;
using RubenDougall.Utilities;

namespace NumberTileGame
{
    internal class Game
    {
        private const int PlayerCount = 2;

        private IList<Player> players = new List<Player>(PlayerCount);
        private IList<Tile> tiles = new List<Tile>(((NumberTile.MaxValue - NumberTile.MinValue + 1) * 8) + 2);
        private LinkedList<Set> sets = new LinkedList<Set>();
        private Random random = new Random();

        public void Setup()
        {
            InputPlayerNames();
            GenerateTiles();
            GivePlayersTiles();
        }

        private void InputPlayerNames()
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

        private void GenerateTiles()
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
        }

        private void GivePlayersTiles()
        {
            foreach (Player player in players)
            {
                for (int i = 0; i < Player.TileCount; i++)
                {
                    GivePlayerTile(player);
                }
            }
        }

        public void Play()
        {
            Player player = players[0];
            Console.WriteLine($"{player.Name.ToUpper()}'s TURN");
            Console.WriteLine();
            Console.WriteLine("Sets:");
            Console.WriteLine(sets.ToHumanReadableString());
            Console.WriteLine();
            Console.WriteLine("Your tiles:");
            Console.WriteLine(player.Tiles.ToHumanReadableString());
            Console.WriteLine();
            
            int option = ConsoleUtilities.ReadOptionInt(new[] {"Edit sets", "Pass and take tile"}, "Choose an option:");
            switch (option)
            {
                case 0:
                    EditSets();
                    break;
                case 1:
                    if (tiles.Count == 0)
                    {
                        Console.WriteLine("No more tiles left, continuing anyway"); // TODO: Change at some point so the above option doesn't display "...take tile"
                    }
                    else
                    {
                        GivePlayerTile(player);
                    }
                    break;
                default:
                    break;
            }
        }

        private void EditSets()
        {
            throw new NotImplementedException();
        }

        private bool CheckPlayerNameDistinctness(string playerName)
        {
            IList<string> playerNames = players.Select(player => player.Name.ToLower()).ToList();
            playerNames.Add(playerName.ToLower());
            Console.WriteLine(playerNames.Distinct().Count());
            return playerNames.Distinct().Count() == playerNames.Count;
        }

        private void GivePlayerTile(Player player)
        {
            int index = random.Next(tiles.Count);
            Tile tile = tiles[index];
            player.Tiles.Add(tile);
            tiles.RemoveAt(index);
        }
    }
}