package uk.co.ruben9922.numbergame;

import java.util.Random;
import java.util.Scanner;

public class Main {
    public static void main(String[] args) {
        Scanner scanner = new Scanner(System.in);
        Random random = new Random();
        NumberGame numberGame = new NumberGame();

        // Print welcome message
        System.out.println("Number Game\n");

        numberGame.inputPlayers(scanner);

        numberGame.generateTiles(2, 1, 13, new SmileyTile(Tile.Colour.BLACK), new SmileyTile(Tile.Colour.ORANGE));

        numberGame.givePlayersTiles(random, 14);

        numberGame.playGame();
    }
}
