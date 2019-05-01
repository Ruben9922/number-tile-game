package uk.co.ruben9922.numbergame;

import org.jetbrains.annotations.NotNull;

import java.util.Iterator;
import java.util.LinkedList;
import java.util.List;
import java.util.ListIterator;

class Set {
    @NotNull
    private List<Tile> tiles;

    public Set() {
        this.tiles = new LinkedList<>();
    }

    public Set(@NotNull List<Tile> tiles) {
        this.tiles = tiles;
    }

    public List<Tile> getTiles() {
        return tiles;
    }

    @Override
    public String toString() {
        StringBuilder stringBuilder = new StringBuilder();
        Iterator<Tile> iterator = tiles.iterator();
        while (iterator.hasNext()) {
            Tile tile = iterator.next();
            stringBuilder.append(tile.toString());

            if (iterator.hasNext()) { // If not last item
                stringBuilder.append(", ");
            }
        }
        return stringBuilder.toString();
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
            if (tile.getTileType() == Tile.TileType.NUMBER) {
                referenceNumberTile = (NumberTile) tile; // Will try to find a way not using casts
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
            if (tile.getTileType() == Tile.TileType.NUMBER) {
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

    public boolean isValid() {
        return determineSetType() == SetType.INVALID;
    }

    public enum SetType {
        INVALID, RUN, GROUP
    }
}
