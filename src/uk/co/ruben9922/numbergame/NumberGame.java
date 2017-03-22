package uk.co.ruben9922.numbergame;

import org.jetbrains.annotations.NotNull;
import uk.co.ruben9922.utilities.consoleutilities.InputUtilities;

import java.util.*;

public class NumberGame {
    public static void main(String[] args) {
        Scanner scanner = new Scanner(System.in);
        Random random = new Random();

        // Welcome message
        System.out.println("Number Game");
        System.out.println();

        List<Player> players = createPlayers(scanner);

        List<Tile> tiles = generateTiles(2, 1, 13, Arrays.asList(new Tile[] {
                new SmileyTile(Tile.Colour.BLACK),
                new SmileyTile(Tile.Colour.ORANGE)
        }));

        // Give players tiles
        for (Player player : players) {
            givePlayerTiles(random, player, tiles, 14);
        }
    }

    @NotNull
    private static List<Player> createPlayers(Scanner scanner) {
        // Input number of players
        final int PLAYER_COUNT = InputUtilities.inputInt(scanner, "Number of players: ", 0, null);
        scanner.nextLine();

        // Initialise array then, for each player, input name, create Player object and add object to array
        Player[] playerArray = new Player[PLAYER_COUNT];
        for (int i = 0; i < playerArray.length; i++) {
            boolean unique;
            do {
                System.out.format("Player %1$d's name (leave blank for \"Player %1$d\"): ", i + 1);
                String playerName = scanner.nextLine().trim();

                unique = true;
                if (playerName.isEmpty()) {
                    // Implementing the "leave blank for ..." from prompt above
                    playerName = "Player " + (i + 1);
                } else {
                    // Check for uniqueness
                    for (int j = 0; j < i; j++) {
                        String existingPlayerName = playerArray[j].getName();
                        if (playerName.toLowerCase().equals(existingPlayerName.toLowerCase())) {
                            unique = false;
                            System.out.format("The name %s is already taken! Enter a different name.\n", existingPlayerName); // Might move this later
                            break;
                        }
                    }
                }

                // If player name is unique, create new Player, add it to array and print success message
                if (unique) {
                    playerArray[i] = new Player(playerName);
                    System.out.format("Player \"%s\" added.\n\n", playerName);
                }
            } while (!unique);
        }

        return Arrays.asList(playerArray);
    }

    // minTileNumber and maxTileNumber are both inclusive
    private static List<Tile> generateTiles(int numberTileCopies, int minTileNumber, int maxTileNumber, List<Tile> extraTiles) {
        Tile.Colour[] colourValues = Tile.Colour.values();
        List<Tile> tiles = new LinkedList<>();

        for (Tile.Colour colour : colourValues) {
            for (int i = 0; i < numberTileCopies; i++) {
                for (int j = minTileNumber; j <= maxTileNumber; j++) {
                    tiles.add(new NumberTile(colour, j));
                }
            }
        }

        for (Tile tile : extraTiles) {
            tiles.add(tile);
        }

        return tiles;
    }

    private static void givePlayerTiles(Random random, Player player, List<Tile> tiles, int tileCount) {
        for (int i = 0; i < tileCount; i++) {
            int index = random.nextInt(tiles.size());
            Tile tile = tiles.remove(index);
            player.getTiles().add(tile);
        }
    }
}
