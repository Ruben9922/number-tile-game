namespace NumberTileGame
{
    internal class SmileyTile : Tile
    {
        public SmileyTile(Colour colour) : base(colour)
        {
        }

        public override string ToString()
        {
            return $"{Colour} Smiley";
        }
    }
}