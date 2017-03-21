package uk.co.ruben9922.numbergame;

import org.jetbrains.annotations.NotNull;
import uk.co.ruben9922.utilities.consoleutilities.InputUtilities;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;
import java.util.Scanner;

public class NumberGame {
    public static void main(String[] args) {
        Scanner scanner = new Scanner(System.in);

        // Welcome message
        System.out.println("Number Game");
        System.out.println();

        List<Player> players = createPlayers(scanner);

        // Simply print player names for testing
        for (Player player : players) {
            System.out.println(player.getName());
        }

        List<Tile> tiles = generateTiles(2, 1, 13, Arrays.asList(new Tile[] {
                new SmileyTile(Tile.Colour.BLACK),
                new SmileyTile(Tile.Colour.ORANGE)
        }));

        // Print all tiles for testing
        for (Tile tile : tiles) {
            System.out.println(tile.toString());
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
                    String existingPlayerName = "";
                    for (int j = 0; j < i; j++) {
                        existingPlayerName = playerArray[j].getName();
                        if (playerName.toLowerCase().equals(existingPlayerName.toLowerCase())) {
                            unique = false;
                            break;
                        }
                    }
                    if (!unique) {
                        System.out.format("The name %s is already taken! Enter a different name.\n", existingPlayerName);
                    }
                }
                playerArray[i] = new Player(playerName);
            } while (!unique);
        }
        System.out.println();

        return Arrays.asList(playerArray);
    }

    // minTileNumber and maxTileNumber are both inclusive
    private static List<Tile> generateTiles(int numberTileCopies, int minTileNumber, int maxTileNumber, List<Tile> extraTiles) {
        Tile.Colour[] colourValues = Tile.Colour.values();
        List<Tile> tiles = new ArrayList<>((numberTileCopies * colourValues.length) + extraTiles.size());

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
}
