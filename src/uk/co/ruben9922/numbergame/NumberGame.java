package uk.co.ruben9922.numbergame;

import org.jetbrains.annotations.Nullable;
import uk.co.ruben9922.utilities.consoleutilities.InputUtilities;

import java.util.Arrays;
import java.util.LinkedList;
import java.util.List;
import java.util.Random;
import java.util.Scanner;

class NumberGame {
    private List<Player> players = new LinkedList<>();
    private List<Tile> tiles = new LinkedList<>(); // Tiles not yet given to a player // TODO: Possibly change tile and set lists to array lists
    private List<Set> sets = new LinkedList<>(); // TODO: Possibly change to use set rather than list

    public NumberGame() {

    }

    private static void moveTile(Scanner scanner, List<Tile> sourceTileList, List<Tile> destinationTileList) {
        // Choose tile to move
        System.out.println("Choose tile to move from source list");
        int tileIndex = ListUtilities.chooseItem(scanner, sourceTileList, "Number of tile to move: ");
        System.out.println();

        // Remove selected tile
        Tile tile = sourceTileList.remove(tileIndex);

        // Choose position in destination list at which to insert tile
        System.out.println("Choose position in destination list to insert tile at");
        int tileInsertIndex = ListUtilities.chooseItem(scanner, destinationTileList, "Position to insert tile: ");

        // Insert tile at selected position
        destinationTileList.add(tileInsertIndex, tile);
    }

    private static List<Tile> chooseSourceTileList(Scanner scanner, List<Tile> playerTiles, List<Set> sets) {
        System.out.println("Choose whether to move a tile from your tiles or from an existing set on the table:");
        if (InputUtilities.inputOptionInt(scanner, new String[]{"From my tiles", "From an existing set"}) == 0) {
            return playerTiles;
        } else {
            return chooseSet(scanner, sets).getTiles();
        }
    }

    private static Set chooseSet(Scanner scanner, List<Set> sets) {
        return sets.get(ListUtilities.chooseItem(scanner, sets,
                String.format("Set number [%d..%d]: ", 0, sets.size() - 1)));
    }

    public void inputPlayers(Scanner scanner) {
        // Input number of players
        final int PLAYER_COUNT = InputUtilities.inputInt(scanner, "Number of players: ", 0, null);
        scanner.nextLine();

        for (int i = 0; i < PLAYER_COUNT; i++) {
            String playerName = inputPlayerName(scanner, players);
            players.add(new Player(playerName));
            System.out.format("Player \"%s\" added.\n\n", playerName);
        }
    }

    private String inputPlayerName(Scanner scanner, List<Player> existingPlayers) {
        String playerName;
        boolean unique;
        do {
            System.out.format("Player %1$d's name (leave blank for \"Player %1$d\"): ", existingPlayers.size() + 1);
            playerName = scanner.nextLine().trim();

            // Implementing the "leave blank for ..." from prompt above
            if (playerName.isEmpty()) {
                playerName = "Player " + (existingPlayers.size() + 1);
            }

            // Check for uniqueness
            String existingPlayerName = findMatchingPlayerName(existingPlayers, playerName);
            unique = (existingPlayerName == null);
            if (!unique) {
                System.out.format("The name %s is already taken! Enter a different name.\n", existingPlayerName); // Might move this later
            }
        } while (!unique);
        return playerName;
    }

    @Nullable
    private String findMatchingPlayerName(List<Player> existingPlayers, String playerName) {
        for (Player player : existingPlayers) {
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

    public void givePlayersTiles(Random random, int tileCount) { // TODO: Rename
        System.out.format("Giving each player %d tiles...\n\n", tileCount);
        for (Player player : players) {
            givePlayerTiles(random, player, tiles, tileCount);
        }
    }

    private void givePlayerTiles(Random random, Player player, List<Tile> tiles, int tileCount) { // TODO: Make static or inline (?)
        for (int i = 0; i < tileCount; i++) {
            int index = random.nextInt(tiles.size());
            Tile tile = tiles.remove(index);
            player.getTiles().add(tile);
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
            List<Tile> sourceTileList = chooseSourceTileList(scanner, updatedPlayerTiles, updatedSets);
            List<Tile> destinationTileList = chooseSet(scanner, updatedSets).getTiles();
            moveTile(scanner, sourceTileList, destinationTileList);

            valid = areAllSetsValid();
            if (valid) {
                System.out.println("Sets are currently VALID");
                System.out.println("Your changes will be SAVED if you choose not to continue editing sets");
            } else {
                System.out.println("Sets are currently INVALID");
                System.out.println("Your changes will be LOST if you choose not to continue editing sets");
            }
        } while (InputUtilities.inputYOrN(scanner, "Continue editing sets? (y/n): "));

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
