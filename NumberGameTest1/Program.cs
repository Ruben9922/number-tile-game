using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RubenConsoleLib;

namespace NumberGameTest1
{
    class Program
    {
        static Random random1 = new Random();
        static List<Tile> tiles = new List<Tile>();

        static void Main(string[] args)
        {
            const int startTileCount = 14; // Number of tiles to initially give each player

            Console.WriteLine("Welcome to Ruben's Number Game Program!");
            Console.WriteLine("Written in August 2015");
            Console.WriteLine();

            // Get number of players
            int playerCount = RubenConsoleMethods.InputInt("Enter number of players: ");
            Console.WriteLine("Starting game with {0} player(s)...", playerCount); 
            Console.WriteLine();
            List<Player> players = new List<Player>();
            List<Set> tableSets = new List<Set>();
            // Generate all tiles
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 2; j++) // Generate 2 copies of each number tile
                {
                    for (int k = 1; k <= 13; k++)
                    {
                        tiles.Add(new Tile(k, i));
                    }
                }
            }
            tiles.Add(new Tile(0, (int)Tile.Colours.Black)); // Generate smiley tiles
            tiles.Add(new Tile(0, (int)Tile.Colours.Orange));
            foreach (Tile tile1 in tiles)
            {
                tile1.DisplayTileString();
                Console.WriteLine();
            }
            // Ask user for player names
            for (int i = 0; i < playerCount; i++)
            {
                Console.Write("Enter player name for Player {0} (optional): ", (i + 1).ToString());
                string name = Console.ReadLine();
                if (name == string.Empty)
                {
                    name = string.Format("Player {0}", (i + 1).ToString());
                }
                players.Add(new Player(name));
            }
            Console.WriteLine();
            // Give each player a certain number of tiles, removing
            // those tiles from the original tile list in the process
            Console.WriteLine("Giving each player {0} tiles at random...", startTileCount);
            foreach (Player player1 in players)
            {
                for (int i = 0; i < startTileCount; i++)
                {
                    int randomNumber = random1.Next(tiles.Count());
                    player1.Tiles.Add(tiles[randomNumber]);
                    tiles.RemoveAt(randomNumber);
                }
            }

            // Play game - loop for each player - need to change this!
            for (int i = 0; i < playerCount; i++)
            {
                Console.WriteLine("{0}'S TURN", players[i].Name.ToUpper());
                // Display tiles currently on table
                Console.WriteLine("Here are the tiles currently on the table:");
                int tableSetCount = tableSets.Count();
                if (tableSetCount == 0)
                {
                    Console.WriteLine("(None)");
                }
                else
                {
                    for (int j = 0; j < tableSetCount; j++)
                    {
                        Console.Write("  Set {0}: ", j);
                        DisplaySet(tableSets[j]);
                        if (j != tableSetCount - 1)
                        {
                            Console.WriteLine();
                        }
                    }
                }
                Console.WriteLine();
                // Display player's tiles
                Console.WriteLine("Here are your tiles:");
                int playerTileCount = players[i].Tiles.Count();
                for (int j = 0; j < playerTileCount; j++)
                {
                    Console.Write("  Tile {0}: ", j);
                    players[i].Tiles[j].DisplayTileString();
                    if (j != playerTileCount - 1)
                    {
                        Console.WriteLine();
                    }
                }
                Console.WriteLine();
                Console.WriteLine("Here are your options:");
                string[] optionsArray1 = new string[]
                {
                    "Create a new set",
                    "Change an existing set",
                    "Skip your turn, and pick up an extra tile"
                };
                int option1 = RubenConsoleMethods.InputOptionInt(optionsArray1);
                switch (option1)
                {
                    case 0:
                        bool tryAgain;
                        do
                        {
                            Console.WriteLine("CREATE NEW SET");
                            int newSetTileCount = RubenConsoleMethods.InputInt("Number of tiles to put into this set: ");
                            Console.WriteLine("Look at your tiles above - each of them has a number (Tile 1, Tile 2, Tile 3, etc.)");
                            Console.WriteLine("You need to enter the numbers of the tiles you want to put into the set, in order");
                            Set newSet = new Set(new List<Tile>());
                            for (int j = 0; j < newSetTileCount; j++)
                            {
                                Console.WriteLine();
                                int newTileIndex = RubenConsoleMethods.InputInt(string.Format("New Set Tile {0}: ", j));
                                newSet.Tiles.Add(players[i].Tiles[newTileIndex]);
                                DisplaySet(newSet);
                            }
                            if (CheckIfSetValid(newSet))
                            {
                                tableSets.Add(newSet);
                                Console.WriteLine("Set successfully added to table!");
                                tryAgain = false;
                                // Remove tiles from player's tile list only if new set is valid
                                foreach (Tile tile1 in newSet.Tiles)
                                {
                                    players[i].Tiles.Remove(tile1);
                                }
                            }
                            else
                            {
                                Console.WriteLine("The set you entered is invalid!");
                                Console.Write("Try again? Enter y or n: ");
                                tryAgain = Console.ReadLine().ToLower() == "y";
                            }
                        } while (tryAgain);
                        break;
                    case 1:
                        Console.WriteLine("EDIT EXISTING SET");

                        break;
                    case 2:
                        Console.WriteLine("TURN SKIPPED");
                        Console.WriteLine("Giving you an extra tile...");
                        GivePlayerTile(players[i]);
                        break;
                }
                Console.Write("Press any key to continue");
                Console.ReadKey();
                Console.WriteLine();
            }

            Console.Write("Press any key to exit ");
            Console.ReadKey();
        }

        private static void GivePlayerTile(Player player1)
        {
            int randomNumber = random1.Next(tiles.Count());
            player1.Tiles.Add(tiles[randomNumber]);
            tiles.RemoveAt(randomNumber);
        }

        private static bool CheckIfSetValid(Set set1)
        {
            bool run = true;
            int col1 = set1.Tiles[0].Colour;
            int num1 = set1.Tiles[0].Colour;
            for (int i = 1; i < set1.Tiles.Count() - 1; i++)
            {
                if (set1.Tiles[i].Colour != col1 || set1.Tiles[i].Number != num1 + i)
                {
                    run = false;
                    break;
                }
            }
            if (run)
            {
                set1.IsRun = true;
                return true;
            }
            bool group = true;
            num1 = set1.Tiles[0].Number;
            for (int i = 1; i < set1.Tiles.Count(); i++)
            {
                if (set1.Tiles[i].Number != num1)
                {
                    group = false;
                    break;
                }
            }
            set1.IsRun = false;
            return group;
        }

        private static void DisplaySet(Set set1)
        {
            int setTileCount = set1.Tiles.Count();
            for (int i = 0; i < setTileCount; i++)
            {
                set1.Tiles[i].DisplayTileString();
                if (i != setTileCount - 1)
                {
                    Console.Write(", ");
                }
            }
        }
    }
}
