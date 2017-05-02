package uk.co.ruben9922.numbergame;

import org.jetbrains.annotations.Nullable;
import uk.co.ruben9922.utilities.consoleutilities.InputUtilities;

import java.util.*;

class NumberGame {
    private List<Player> players = new LinkedList<>();
    private List<Tile> tiles = new LinkedList<>(); // Tiles not yet given to a player

    public NumberGame() {

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

            unique = true;

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

    public void givePlayersTiles(Random random, int tileCount) {
        System.out.format("Giving each player %d tiles...\n\n", tileCount);
        for (Player player : players) {
            givePlayerTiles(random, player, tiles, tileCount);
        }
    }

    private void givePlayerTiles(Random random, Player player, List<Tile> tiles, int tileCount) {
        for (int i = 0; i < tileCount; i++) {
            int index = random.nextInt(tiles.size());
            Tile tile = tiles.remove(index);
            player.getTiles().add(tile);
        }
    }

    public void playGame() {
        for (Player player : players) {
            System.out.format("%s's Turn\n", player.getName());
            System.out.println("Tiles: ");
            printList(player.getTiles(), true);

            System.out.println();
        }
    }

    public static <E> void printList(List<E> tiles, boolean showIndices) {
        ListIterator<E> listIterator = tiles.listIterator();
        while (listIterator.hasNext()) {
            E element = listIterator.next();
            int index = listIterator.nextIndex();
            if (showIndices) {
                System.out.format("[%d] ", index);
            }
            System.out.format("%s\n", element.toString());
        }
    }
}
