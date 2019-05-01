using System;
using System.Collections.Generic;
using System.Linq;
using Ruben9922.Utilities.ConsoleUtilities;

namespace NumberGameTest1
{
    class Program
    {
        private static Random random = new Random();
        private const string Separator = "---------------";
        private const string InvalidOptionMessage = "Invalid option";

        static void Main(string[] args)
        {
            List<Set> sets = new List<Set>(new Set[] {
                new Set(new List<Tile>(new Tile[]
                {
                    new Tile(0, (int)Tile.Colours.Orange),
                    new Tile(5, (int)Tile.Colours.Black),
                    new Tile(6, (int)Tile.Colours.Black)
                })),
                new Set(new List<Tile>(new Tile[]
                {
                    new Tile(4, (int)Tile.Colours.Black),
                    new Tile(5, (int)Tile.Colours.Black),
                    new Tile(6, (int)Tile.Colours.Black)
                }))
            }); // Could use Count property instead of Count method for lists as specifically using lists

            // Set tests
            //Set[] testSets = new Set[]
            //{
            //    new Set(new List<Tile>(new Tile[]
            //    {
            //        new Tile(0, (int)Tile.Colours.Black),
            //        new Tile(5, (int)Tile.Colours.Black),
            //        new Tile(6, (int)Tile.Colours.Black)
            //    })),
            //    new Set(new List<Tile>(new Tile[]
            //    {
            //        new Tile(0, (int)Tile.Colours.Orange),
            //        new Tile(5, (int)Tile.Colours.Black),
            //        new Tile(6, (int)Tile.Colours.Black)
            //    })),
            //    new Set(new List<Tile>(new Tile[]
            //    {
            //        new Tile(1, (int)Tile.Colours.Orange),
            //        new Tile(2, (int)Tile.Colours.Orange),
            //        new Tile(3, (int)Tile.Colours.Orange)
            //    })),
            //    new Set(new List<Tile>(new Tile[]
            //    {
            //        new Tile(0, (int)Tile.Colours.Blue),
            //        new Tile(0, (int)Tile.Colours.Red),
            //        new Tile(0, (int)Tile.Colours.Orange)
            //    })),
            //    new Set(new List<Tile>(new Tile[]
            //    {
            //        new Tile(4, (int)Tile.Colours.Red),
            //        new Tile(0, (int)Tile.Colours.Red),
            //        new Tile(5, (int)Tile.Colours.Red)
            //    })),
            //    new Set(new List<Tile>(new Tile[]
            //    {
            //        new Tile(0, (int)Tile.Colours.Blue),
            //        new Tile(4, (int)Tile.Colours.Red),
            //        new Tile(4, (int)Tile.Colours.Orange)
            //    }))
            //};
            //foreach (Set set in testSets)
            //{
            //    set.UpdateSetType();
            //    DisplaySet(set);
            //    Console.WriteLine();
            //}

            const int InitialPlayerTileCount = 14; // Number of tiles to initially give each player
            const int NumberTileCopies = 2;
            const int HighestNumberTile = 13;

            List<Tile> tiles = GenerateTiles(NumberTileCopies, HighestNumberTile);
            int maxPlayerCount = tiles.Count() / InitialPlayerTileCount; // Maximum number of players to allow
            List<Player> players = new List<Player>();

            Console.WriteLine("Welcome to Ruben's Number Game Program!");
            Console.WriteLine();

            // Input number of players
            int playerCount = ConsoleReadUtilities.ReadInt("Number of players: ", 1, maxPlayerCount + 1, string.Format("Integers between 1 and {0} inclusive only!", maxPlayerCount));
            Console.WriteLine();

            // Display tiles for testing purposes
            //Console.ForegroundColor = ConsoleColor.White;
            //foreach (Tile tile in tiles)
            //{
            //    Console.WriteLine(tile.ToString());
            //}
            //Console.ResetColor();
            //Console.WriteLine();

            // Input player names and create Player objects
            // Could check for uniqueness
            for (int i = 0; i < playerCount; i++)
            {
                Console.Write("Name of player {0} (leave blank for \"Player {0}\"): ", i + 1);
                string name = Console.ReadLine();
                // If no name entered, set name as Player X, where X is 1-based index of player (e.g. Player 1 for first player).
                if (string.IsNullOrWhiteSpace(name))
                {
                    name = string.Format("Player {0}", i + 1);
                }
                players.Add(new Player(name));
            }
            Console.WriteLine();

            // Give each player a certain number of tiles - could merge with for loop above
            Console.WriteLine("Giving each player {0} tiles at random", InitialPlayerTileCount);
            bool tilesLeft = true;
            for (int i = 0; i < playerCount && tilesLeft; i++)
            {
                for (int j = 0; j < InitialPlayerTileCount && tilesLeft; j++)
                {
                    tilesLeft = GivePlayerTile(tiles, players[i], false);
                }
            }
            Console.WriteLine();
            Console.WriteLine(Separator);
            Console.WriteLine();

            // Play game - loop for each player - need to change this!
            for (int i = 0; i < playerCount; i++)
            {
                Player currentPlayer = players[i];

                // Only take turn if player is still in game (i.e. hasn't used all of their tiles)
                if (currentPlayer.IsPlaying)
                {
                    TakeTurn(ref sets, tiles, currentPlayer);
                }
                else
                {
                    Console.WriteLine("{0}'s turn was skipped as they've used up their tiles", currentPlayer.Name);
                }

                DisplayPlayerTiles(currentPlayer);
                Console.WriteLine();

                Console.Write("Press any key to continue...");
                Console.ReadKey();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine(Separator);
                Console.WriteLine();
            }

            Console.WriteLine("Game finished!");
            Console.Write("Press any key to exit...");
            Console.ReadKey();
        }

        private static List<Tile> GenerateTiles(int numberTileCopies, int highestNumberTile)
        {
            List<Tile> tiles = new List<Tile>((4 * numberTileCopies * highestNumberTile) + 2);

            // Generate all tiles; 
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < numberTileCopies; j++) // Generate multiple copies of each number tile
                {
                    for (int k = 1; k <= highestNumberTile; k++)
                    {
                        tiles.Add(new Tile(k, i));
                    }
                }
            }

            // Generate smiley tiles
            tiles.Add(new Tile(0, (int)Tile.Colours.Black));
            tiles.Add(new Tile(0, (int)Tile.Colours.Orange));

            return tiles;
        }

        private static void TakeTurn(ref List<Set> sets, List<Tile> tiles, Player currentPlayer)
        {
            Console.WriteLine("{0}'S TURN", currentPlayer.Name.ToUpper());
            Console.WriteLine();

            // Display available moves, input move and call appropriate method
            bool moveAgain = false;
            do
            {
                DisplaySets(sets);
                Console.WriteLine();

                DisplayPlayerTiles(currentPlayer);
                Console.WriteLine();

                Console.WriteLine("Your options:");
                int moveOption = ConsoleReadUtilities.ReadOptionInt(new string[] // Could add option to return at each stage
                {
                            "Create new set",
                            "Edit existing sets",
                            moveAgain ? "End your turn" : "Skip your turn, and pick up an extra tile" // Display different message depending on whether this is first move (in this turn) or not
                });
                Console.WriteLine();
                Console.WriteLine(Separator);
                Console.WriteLine();
                switch (moveOption)
                {
                    case 0:
                        Console.WriteLine("CREATE NEW SET"); // Will at some point merge with editing sets functionality
                        Console.WriteLine();

                        Set newSet = CreateSet(currentPlayer);
                        if (newSet == null)
                        {
                            moveAgain = true;
                        }
                        else
                        {
                            sets.Add(newSet);
                            Console.WriteLine("New set created and added to table!");
                            DisplaySet(newSet);
                            Console.WriteLine();
                            Console.WriteLine(Separator);
                            Console.WriteLine();
                            moveAgain = ConsoleReadUtilities.ReadYOrN("Make any more moves? (y/n): ");
                        }

                        break;
                    case 1:
                        moveAgain = EditSets(currentPlayer, ref sets);

                        break;
                    case 2:
                        if (moveAgain)
                        {
                            Console.WriteLine("TURN ENDED");
                        }
                        else
                        {
                            Console.WriteLine("TURN SKIPPED");
                            Console.WriteLine("Giving {0} an extra tile", currentPlayer.Name);
                            GivePlayerTile(tiles, currentPlayer, true);
                        }
                        moveAgain = false;

                        break;
                    default:
                        Console.WriteLine(InvalidOptionMessage);
                        moveAgain = true;

                        break;
                }
                Console.WriteLine();
                Console.WriteLine(Separator);
                Console.WriteLine();
            } while (moveAgain);
        }

        private static void DisplaySet(Set set) // Might refactor
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(set.ToString());
            Console.ResetColor();
            Console.Write(" (");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(set.SetTypeToString());
            Console.ResetColor();
            Console.WriteLine(")");
        }

        // Display tiles currently on table
        private static void DisplaySets(List<Set> sets) // Could refactor this
        {
            Console.WriteLine("Sets:");

            int setCount = sets.Count();
            if (setCount == 0)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("  (None)");
            }
            else
            {
                for (int j = 0; j < setCount; j++)
                {
                    Set set = sets[j];
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("  ({0}): ", j);
                    DisplaySet(set);
                }
            }

            Console.ResetColor();
        }

        // Display player's tiles
        private static void DisplayPlayerTiles(Player player)
        {
            Console.WriteLine("Your tiles:");
            Console.ForegroundColor = ConsoleColor.Yellow;
            for (int j = 0; j < player.Tiles.Count(); j++)
            {
                Console.WriteLine("  ({0}): {1}", j, player.Tiles[j].ToString());
            }
            Console.ResetColor();
        }

        // Returns whether successful (i.e. whether there was at least one tile in given tiles list)
        private static bool GivePlayerTile(List<Tile> tiles, Player player, bool displayNewTile)
        {
            if (tiles.Count() > 0) // May change
            {
                // Add a tile from main tiles list to player's tiles list and remove that tile from main tiles list
                int tileIndex = random.Next(tiles.Count());
                Tile newTile = tiles[tileIndex];
                player.Tiles.Add(newTile);
                tiles.RemoveAt(tileIndex);
                if (displayNewTile)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(newTile.ToString());
                    Console.ResetColor();
                }
                return true;
            }
            else
            {
                if (displayNewTile)
                {
                    Console.WriteLine("Failed to give player a tile. Out of tiles!");
                }
                return false;
            }
        }

        // Inputs new set
        // If a valid set is eventually entered, returns that set; otherwise returns null
        private static Set CreateSet(Player player)
        {
            // Repeat until valid set is entered
            bool tryAgain;
            do
            {
                // Input number of tiles to put into set
                int newSetTileCount = ConsoleReadUtilities.ReadInt("Number of tiles to put into this set: ", Math.Max(1, Set.MinSize), null, string.Format("Integers greater than or equal to {0} only!", Math.Max(1, Set.MinSize)));
                Console.WriteLine();

                // Input which tiles on table should be put into new set
                Console.WriteLine("Each tile listed above has a number in parentheses - e.g. (0), (1), (2)");
                Console.WriteLine("Enter the numbers of the tiles to put into the set, in order");
                Console.WriteLine();
                List<Tile> newSetTiles = new List<Tile>();
                for (int j = 0; j < newSetTileCount; j++)
                {
                    int playerTilesCount = player.Tiles.Count();
                    int newTileIndex = ConsoleReadUtilities.ReadInt(string.Format("New set tile {0}: ", j), 0, playerTilesCount, string.Format("Integers between 0 and {0} inclusive only!", playerTilesCount - 1));
                    newSetTiles.Add(player.Tiles[newTileIndex]);

                    // Display new set tiles so far
                    Console.ForegroundColor = ConsoleColor.White;
                    newSetTiles.Display();
                    Console.ResetColor();
                    Console.WriteLine();
                }

                // Check if new set is valid
                // If so remove tiles from player's tiles; otherwise display message and ask user whether to try again
                Set newSet = new Set(newSetTiles);
                newSet.UpdateSetType();
                if (newSet.IsValid())
                {
                    tryAgain = false;
                    foreach (Tile tile1 in newSet.Tiles)
                    {
                        player.Tiles.Remove(tile1);
                    }
                    return newSet;
                }
                else
                {
                    Console.WriteLine("The set you entered is invalid!");
                    tryAgain = ConsoleReadUtilities.ReadYOrN();
                    Console.WriteLine();
                }
            } while (tryAgain);
            return null;
        }

        private static bool EditSets(Player player, ref List<Set> sets)
        {
            if (sets.Count() > 0) // Will remove after merging creating sets with this
            {
                Console.WriteLine("EDIT EXISTING SETS");
                Console.WriteLine();
                Console.WriteLine("Move tiles, from either your tiles or an existing set, to another set");
                Console.WriteLine("Move as many tiles as you like");
                Console.WriteLine("Changes will only be saved if all sets are valid");
                Console.WriteLine();

                // Allow player to transfer tiles, between sets or from their tiles into a set, as many times as they like
                List<Set> tempSets = sets.Clone();
                List<Tile> tempPlayerTiles = player.Tiles.Clone();
                bool valid;
                do
                {
                    valid = MoveTile(tempSets, tempPlayerTiles);

                    // Display whether all sets are valid
                    if (valid)
                    {
                        Console.WriteLine("Sets are currently VALID");
                        Console.WriteLine("Your changes will be SAVED if you choose not to continue editing sets");
                    }
                    else
                    {
                        Console.WriteLine("Sets are currently INVALID");
                        Console.WriteLine("Your changes will be LOST if you choose not to continue editing sets");
                    }
                    Console.WriteLine();

                    // Display sets and player's tiles
                    DisplaySets(tempSets);
                    Console.WriteLine();
                    DisplayPlayerTiles(player);
                    Console.WriteLine();
                } while (ConsoleReadUtilities.ReadYOrN("Continue editing sets? (y/n): "));
                Console.WriteLine();

                // If all sets were valid, overwrite main sets (and player tiles list if it has been edited) with temporary ones
                if (valid)
                {
                    sets = tempSets;
                    if (tempPlayerTiles != null)
                    {
                        player.Tiles = tempPlayerTiles;
                    }
                    Console.WriteLine(Separator);
                    Console.WriteLine();
                    return ConsoleReadUtilities.ReadYOrN("Make any more moves? (y/n): ");
                }
                else
                {
                    return true;
                }
            }
            else
            {
                Console.WriteLine("Cannot edit sets as there are no sets");
                return true;
            }
        }

        // Returns whether sets are valid after moving tile
        private static bool MoveTile(List<Set> sets, List<Tile> playerTiles)
        {
            bool valid;
            List<Tile> sourceTilesList;
            List<Tile> destinationTilesList;
            int removeIndex;
            Tile tileToTransfer;
            int insertIndex;
            string chooseTilePrompt;

            // Choose where to get tiles from
            Console.WriteLine("Choose whether to move tile from your tiles or from an existing set:");
            int tileSourceOption = ConsoleReadUtilities.ReadOptionInt(new string[] {
                "From your tiles",
                "From an existing set on the table"
            });
            Console.WriteLine();
            switch (tileSourceOption)
            {
                case 0:
                    sourceTilesList = playerTiles;
                    chooseTilePrompt = "Tile to move from your own tiles: ";
                    Console.WriteLine("Your tiles:");

                    break;
                case 1:
                    sourceTilesList = sets[sets.ChooseItem("Set #: ")].Tiles;
                    chooseTilePrompt = "Tile to move from selected set: ";
                    Console.WriteLine("Selected set:");

                    break;
                default: // Should never be run
                    Console.WriteLine(InvalidOptionMessage);
                    sourceTilesList = null;
                    tileToTransfer = null;
                    chooseTilePrompt = string.Empty;

                    break;
            }

            // Choose tile from source tile list, choose destination and input position to insert selected tile
            sourceTilesList.Display();
            Console.WriteLine();
            removeIndex = sourceTilesList.ChooseItem(chooseTilePrompt); // Could add ability to select multiple tiles
            tileToTransfer = sourceTilesList[removeIndex];
            Console.WriteLine("Selected tile: {0}", tileToTransfer.ToString());
            Console.WriteLine();
            Set destinationSet = sets[sets.ChooseItem("Destination set #: ")];
            destinationTilesList = destinationSet.Tiles;
            Console.WriteLine();
            Console.WriteLine("Now enter where to place the selected tile into that set:");
            int tempDestinationTilesListCount = destinationTilesList.Count();
            insertIndex = ConsoleReadUtilities.ReadInt(string.Format("Position to place tile (between 0 and {0} inclusive): ", tempDestinationTilesListCount), 0, tempDestinationTilesListCount + 1, string.Format("Integers between 0 and {0} inclusive only!", tempDestinationTilesListCount)); // Could generate error message for min and max values automatically

            // Insert tile into destination tiles list and remove from source tiles list
            destinationTilesList.Insert(insertIndex, tileToTransfer);
            sourceTilesList.RemoveAt(removeIndex);

            // Check if all resulting sets are valid and store result
            valid = true;
            foreach (Set set in sets) // Could change so only modified sets are checked
            {
                set.UpdateSetType();
                if (!set.IsValid())
                {
                    valid = false;
                    break;
                }
            }

            return valid;
        }
    }
}
