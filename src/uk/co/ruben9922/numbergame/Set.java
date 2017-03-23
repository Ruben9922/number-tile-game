package uk.co.ruben9922.numbergame;

import java.util.LinkedList;
import java.util.List;
import java.util.ListIterator;

class Set {
    public enum SetType {
        INVALID, RUN, GROUP
    }

    private List<Tile> tiles;

    public Set() {
        this.tiles = new LinkedList<>();
    }

    public Set(List<Tile> tiles) {
        this.tiles = tiles;
    }

    public List<Tile> getTiles() {
        return tiles;
    }

    public SetType determineSetType() {
        final int MIN_SIZE = 3;

        if (tiles.size() < MIN_SIZE) {
            return SetType.INVALID; // Set is too small
        }

        boolean isRun = true;
        boolean isGroup = true;

        ListIterator<Tile> listIterator = tiles.listIterator();
        NumberTile referenceNumberTile = null;
        while (listIterator.hasNext()) {
            Tile tile = listIterator.next();
            if (tile instanceof NumberTile) { // May change later to not use instanceof
                referenceNumberTile = (NumberTile) tile;
                break;
            }
        }

        if (referenceNumberTile == null) {
            return SetType.RUN;
        }

        int referenceNumberTileIndex = listIterator.previousIndex();
        Tile.Colour referenceColour = referenceNumberTile.getColour();
        int referenceNumber = referenceNumberTile.getNumber();

        while (listIterator.hasNext()) {
            Tile tile = listIterator.next();
            if (tile instanceof NumberTile) {
                NumberTile numberTile = (NumberTile) tile;

                if (numberTile.getColour() != referenceColour || numberTile.getNumber() != referenceNumber
                        + listIterator.previousIndex() - referenceNumberTileIndex + 1) {
                    isRun = false;
                }

                if (numberTile.getNumber() != referenceNumber) {
                    isGroup = false;
                }
            }
        }

        if (isRun) {
            return SetType.RUN;
        }
        if (isGroup) {
            return SetType.GROUP;
        }

        return SetType.INVALID; // Set is long enough but is neither a run nor group
    }
}
