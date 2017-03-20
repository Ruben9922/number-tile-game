package uk.co.ruben9922.numbergame;

class Tile {
    public enum Number {
        SMILEY(0), ONE(1), TWO(2), THREE(3), FOUR(4), FIVE(5), SIX(6), SEVEN(7), EIGHT(8), NINE(9), TEN(10), ELEVEN(11),
        TWELVE(12), THIRTEEN(13);

        private final int value;

        Number(int value) {
            this.value = value;
        }

        public int getValue() {
            return value;
        }
    }

    public enum Colour {
        BLACK, BLUE, ORANGE, RED
    }

    private Number number;
    private Colour colour;

    public Tile(Number number, Colour colour) {
        this.number = number;
        this.colour = colour;
    }

    public Number getNumber() {
        return number;
    }

    public Colour getColour() {
        return colour;
    }
}
