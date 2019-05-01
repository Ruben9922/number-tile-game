package uk.co.ruben9922.numbergame;

import org.jetbrains.annotations.NotNull;
import org.jetbrains.annotations.Nullable;
import uk.co.ruben9922.utilities.consoleutilities.InputUtilities;

import java.util.Arrays;
import java.util.LinkedList;
import java.util.List;
import java.util.Random;
import java.util.Scanner;

// TODO: Sort out messages
// TODO: Make tile lists sorted
// TODO: Remove separate "step" of choosing between new and existing sets
// TODO: Display destination tile list
class NumberGame {
    private List<Player> players = new LinkedList<>();
    private List<Tile> tiles = new LinkedList<>(); // Tiles not yet given to a player // TODO: Possibly change tile and set lists to array lists
    private List<Set> sets = new LinkedList<>(); // TODO: Possibly change to use set rather than list

    public NumberGame() {

    }

    private static void moveTile(Scanner scanner, @NotNull List<Tile> sourceTileList,
                                 @NotNull List<Tile> destinationTileList) {
        // Choose tile to move
        System.out.println("Choose tile to move from source list");
        Integer tileRemoveIndex = ListUtilities.chooseItem(scanner, sourceTileList, "source list", "tile", "Number of tile to move: "
        );

        if (tileRemoveIndex == null) { // Null check
            // Should never be reached as chooseItem would only return null if sourceList is empty, but calling code
            // should only ever provide a non-empty source list
            return;
        }

        System.out.println();

        // Remove selected tile
        Tile tile = sourceTileList.remove(tileRemoveIndex.intValue());

        // Choose position in destination list at which to insert tile
        System.out.println("Choose position in destination list to insert tile at");
        Integer tileInsertIndex = ListUtilities.chooseItem(scanner, destinationTileList, "destination list", "tile", "Position to insert tile: "
        );

        if (tileInsertIndex == null) { // Null check
            tileInsertIndex = 0;
        }

        // Insert tile at selected position
        destinationTileList.add(tileInsertIndex, tile);
    }

    @NotNull
    private static List<Tile> chooseSourceTileList(Scanner scanner, @NotNull List<Set> sets,
                                                   @NotNull List<Tile> playerTiles) {
        // If there are no sets on table then source tile list must be player's tiles
        // If there are sets on table then allow player to choose between their own tiles and existing set on table
        if (sets.size() != 0) {
            System.out.println("Choose whether to move a tile from your tiles or from an existing set on the table");
            int option = InputUtilities.inputOptionInt(scanner, new String[] {"From my tiles", "From an existing set"});
            if (option != 0) {
                // Choose an existing set
                Set set = chooseSet(scanner, sets);
                if (set != null) return set.getTiles();
            }
        }
        return playerTiles;
    }

    @NotNull
    private static List<Tile> chooseDestinationTileList(Scanner scanner, @NotNull List<Set> sets) {
        // If there are no sets on table then destination tile list must be that of a NEW set
        // If there are sets on table then allow player to choose between using a NEW or EXISTING set
        if (sets.size() != 0) {
            System.out.println("Choose whether to move a tile to a new or existing set");
            int option = InputUtilities.inputOptionInt(scanner, new String[] {"New set", "Existing set"});
            if (option != 0) {
                // Choose an existing set
                Set set = chooseSet(scanner, sets);
                if (set != null) return set.getTiles();
            }
        }

        // Create a new set, add it to set list and return its tile list
        Set newSet = new Set();
        sets.add(newSet); // Add new set to given set list
        return newSet.getTiles();
    }

    @Nullable
    private static Set chooseSet(Scanner scanner, @NotNull List<Set> sets) {
        Integer index = ListUtilities.chooseItem(scanner, sets,
                "sets", "set", String.format("Set number [%d..%d]: ", 0, sets.size() - 1));

        if (index == null) { // Null check
            return null;
        }

        return sets.get(index);
    }

    private static void givePlayerTiles(Random random, Player player, List<Tile> tiles, int tileCount) {
        for (int i = 0; i < tileCount; i++) {
            int index = random.nextInt(tiles.size());
            Tile tile = tiles.remove(index);
            player.getTiles().add(tile);
        }
    }

    public void inputPlayers(Scanner scanner) {
        // Input number of players
        final int PLAYER_COUNT = InputUtilities.inputInt(scanner, "Number of players: ", 0, null);
        scanner.nextLine();

        for (int i = 0; i < PLAYER_COUNT; i++) {
            String playerName = inputPlayerName(scanner);
            players.add(new Player(playerName));
            System.out.format("Player \"%s\" added.\n\n", playerName);
        }
    }

    private String inputPlayerName(Scanner scanner) {
        String name;
        boolean unique;
        do {
            String defaultName = "Player " + (players.size() + 1);
            System.out.format("Player %d's name (leave blank for \"%s\"): ", players.size() + 1, defaultName);
            name = scanner.nextLine().trim();

            // Implementing the "leave blank for ..." from prompt above
            if (name.isEmpty()) {
                name = defaultName;
            }

            // Check for uniqueness
            String existingPlayerName = findMatchingPlayerName(name);
            unique = (existingPlayerName == null);
            if (!unique) {
                System.out.format("The name %s is already taken! Enter a different name.\n", existingPlayerName); // Might move this later
            }
        } while (!unique);
        return name;
    }

    @Nullable
    private String findMatchingPlayerName(String playerName) {
        for (Player player : players) {
            if (playerName.toLowerCase().equals(player.getName().toLowerCase())) { // Ignoring case
                return player.getName();
            }
        }
        return null;
    }

    // minTileNumber and maxTileNumber are both inclusive
    public void generateTiles(int numberTileCopies, int minTileNumber, int maxTileNumber, Tile... extraTiles) {
        // Add given number of copies of each colour of each number tile between the given minimum and maximum (incl.)
        for (Tile.Colour colour : Tile.Colour.values()) {
            for (int i = 0; i < numberTileCopies; i++) {
                for (int j = minTileNumber; j <= maxTileNumber; j++) {
                    tiles.add(new NumberTile(colour, j));
                }
            }
        }

        // Add extra tiles (e.g. smiley tiles) to tiles list
        tiles.addAll(Arrays.asList(extraTiles));
    }

    public void giveAllPlayersTiles(Random random, int tileCount) {
        System.out.format("Giving each player %d tiles...\n\n", tileCount);
        for (Player player : players) {
            givePlayerTiles(random, player, tiles, tileCount);
        }
    }

    public void playGame(Scanner scanner) {
        for (Player player : players) {
            System.out.format("%s's Turn\n", player.getName());

            // Display sets "on table"
            printTableTiles();
            System.out.println();

            // Display current player's tiles
            player.printTiles();
            System.out.println();

            // Edit sets
            editSets(scanner, player);
        }
    }

    private void printTableTiles() {
        System.out.println("Tiles currently on table:");
        ListUtilities.printList(sets, true);
    }

    private void editSets(Scanner scanner, Player player) {
        boolean valid;
        List<Tile> updatedPlayerTiles = new LinkedList<>(player.getTiles());
        List<Set> updatedSets = new LinkedList<>(sets);
        do {
            // Choose tile lists to move tiles from and to respectively
            // Choose from updatedSets as don't want to edit
            List<Tile> sourceTileList = chooseSourceTileList(scanner, updatedSets, updatedPlayerTiles);
            List<Tile> destinationTileList = chooseDestinationTileList(scanner, updatedSets);
            moveTile(scanner, sourceTileList, destinationTileList);

            valid = areAllSetsValid();
            if (valid) {
                System.out.println("Sets are currently VALID");
                System.out.println("Your changes will be SAVED if you choose not to continue editing sets");
            } else {
                System.out.println("Sets are currently INVALID");
                System.out.println("Your changes will be LOST if you choose not to continue editing sets");
            }
        } while (InputUtilities.inputYOrN(scanner, "Continue editing sets? (y/n): ")); // TODO: Fix this

        if (valid) {
            // If resulting sets are valid, overwrite player tiles and sets
            player.setTiles(updatedPlayerTiles);
            sets = updatedSets;
        }
    }

    private boolean areAllSetsValid() {
        for (Set set : sets) {
            if (!set.isValid()) {
                return false;
            }
        }
        return true;
    }
}
