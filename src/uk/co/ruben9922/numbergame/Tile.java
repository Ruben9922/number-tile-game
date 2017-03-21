package uk.co.ruben9922.numbergame;

abstract class Tile {
    public enum Colour {
        BLACK, BLUE, ORANGE, RED;

        @Override
        public String toString() {
            String defaultString = super.toString();
            return defaultString.substring(0, 1).toUpperCase() + defaultString.substring(1).toLowerCase();
        }
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
