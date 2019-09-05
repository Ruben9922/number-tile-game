using System;
using System.Collections.Generic;
using System.Linq;

namespace NumberTileGame
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Game game = new Game();
            game.InputPlayerNames();
        }
    }

    internal class Player
    {
        internal string Name { get; set; }

        public Player(string name)
        {
            Name = name;
        }
    }

    internal class Game
    {
        private const int PlayerCount = 2;

        private IList<Player> players = new Player[PlayerCount];

        internal void InputPlayerNames()
        {
            for (int i = 0; i < PlayerCount; i++)
            {
                string playerName;
                do
                {
                    Console.Write($"Name of Player {i + 1} [Player {i + 1}]: ");
                    playerName = Console.ReadLine();
                    if (playerName == "")
                    {
                        playerName = $"Player {i + 1}";
                    }
                } while (!CheckPlayerNameDistinctness());

                players[i] = new Player(playerName);
            }
        }

        private bool CheckPlayerNameDistinctness()
        {
            return players.Select(player => player.Name.ToLower()).Distinct().Count() == players.Count;
        }
    }

    internal abstract class Tile
    {
        public Colour Colour { get; set; }

        protected Tile(Colour colour)
        {
            Colour = colour;
        }
    }

    internal class NumberTile : Tile
    {
        public int Number { get; set; }

        public NumberTile(Colour colour, int number) : base(colour)
        {
            Number = number;
        }
    }

    internal class SmileyTile : Tile
    {
        public SmileyTile(Colour colour) : base(colour)
        {
        }
    }

    internal enum Colour
    {
        Black,
        Blue,
        Orange,
        Red
    }

    internal enum TileType
    {
        Number,
        Smiley
    }
}