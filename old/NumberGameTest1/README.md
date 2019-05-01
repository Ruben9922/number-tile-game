# Number Game
Tile-based number game, written in Visual C#.

## Objective
The objective of the game is to use up your tiles as quickly as possible by using your tiles to form sets of tiles.

## Setup
1.  __Enter the number of players__.
2.  __Enter the name of each player.__ The program will refer to each player using the name entered. If no name is entered for a player, that player's name will be set to "Player _X_", where _X_ is the number of that player (e.g. "Player 1" for the first player).
3.  

## Playing the Game

### Tiles
The program first generates a "bag" of tiles. Tiles either display a number between 1 and 13 inclusive, or a smiley face. Smiley face tiles can be used in place of any number tile.  
(In a future update I may change this allow the user to have more control over which tiles are generated.)
Each player is initially given 14 tiles (though I may allow this number to be changed in a future update).

### Sets
*   Players use tiles currently on the table and, optionally, one or more of their own tiles to form sets of tiles.
*   Each set must consist of at least 3 tiles to be valid.
*   Each set may be a _run_ or _group_.
    *   A run consists of tiles of the same colour and whose numbers are consecutive (e.g. 2, 3 and 4, all of which are blue).
    *   A group consists of tiles 

## Future Updates
* Allow the user to change game settings, such as . This could be done using a sep
