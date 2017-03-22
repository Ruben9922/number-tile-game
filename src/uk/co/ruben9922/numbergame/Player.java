package uk.co.ruben9922.numbergame;

import java.util.LinkedList;
import java.util.List;

class Player {
    private String name;
    private List<Tile> tiles;

    public Player(String name, List<Tile> tiles) {
        this.name = name;
        this.tiles = tiles;
    }

    public Player(String name) {
        this(name, new LinkedList<>());
    }

    public String getName() {
        return name;
    }

    public List<Tile> getTiles() {
        return tiles;
    }
}
