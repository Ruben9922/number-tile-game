package uk.co.ruben9922.numbergame;

import org.jetbrains.annotations.NotNull;
import uk.co.ruben9922.utilities.consoleutilities.InputUtilities;

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

        // Simply output player names for testing
        for (Player player : players) {
            System.out.println(player.getName());
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
            System.out.format("Player %1$d's name (leave blank for \"Player %1$d\"): ", i + 1);
            String playerName = scanner.nextLine().trim();
            if (playerName.isEmpty()) {
                playerName = "Player " + (i + 1);
            }
            playerArray[i] = new Player(playerName);
        }

        return Arrays.asList(playerArray);
    }
}
