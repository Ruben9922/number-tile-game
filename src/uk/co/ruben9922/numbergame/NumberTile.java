package uk.co.ruben9922.numbergame;

public class NumberTile extends Tile {
    private int number;

    public NumberTile(Colour colour, int number) {
        super(colour);
        this.number = number;
    }

    public int getNumber() {
        return number;
    }

    @Override
    public String toString() {
        return colour.toString() + " " + number;
    }
}
