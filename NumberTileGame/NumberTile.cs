namespace NumberTileGame
{
    internal class NumberTile : Tile
    {
        public int Number { get; set; }

        public const int MinValue = 1;
        public const int MaxValue = 13;

        public NumberTile(Colour colour, int number) : base(colour)
        {
            Number = number;
        }

        public override string ToString()
        {
            return $"NumberTileGame.NumberTile(Number={Number},Colour={Colour})";
        }
    }
}