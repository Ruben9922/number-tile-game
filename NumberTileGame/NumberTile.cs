namespace NumberTileGame
{
    internal class NumberTile : Tile
    {
        public int Number { get; set; }

        public NumberTile(Colour colour, int number) : base(colour)
        {
            Number = number;
        }
    }
}