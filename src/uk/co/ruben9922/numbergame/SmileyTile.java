package uk.co.ruben9922.numbergame;

class SmileyTile extends Tile {
    public SmileyTile(Colour colour) {
        super(colour);
    }

    @Override
    public String toString() {
        return colour.toString() + " " + "Smiley";
    }
}
