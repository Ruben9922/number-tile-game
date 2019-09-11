namespace NumberTileGame
{
    internal abstract class Tile
    {
        public Colour Colour { get; set; }

        protected Tile(Colour colour)
        {
            Colour = colour;
        }

        public override string ToString()
        {
            return $"{Colour} Tile";
        }
    }
}