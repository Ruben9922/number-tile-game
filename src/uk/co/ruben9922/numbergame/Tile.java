package uk.co.ruben9922.numbergame;

abstract class Tile {
    public enum Colour {
        BLACK, BLUE, ORANGE, RED
    }

    protected Colour colour;

    public Tile(Colour colour) {
        this.colour = colour;
    }

    public Colour getColour() {
        return colour;
    }

    @Override
    public abstract String toString();
}
