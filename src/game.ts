import {getEnumLength} from "./utilities";
import * as R from "remeda";
import {cartesianProduct} from "cartesian-product-multiple-arrays";
import shuffle from "shuffle-array";
import Player from "./player";
import Tile from "./tile";
import Set from "./set";
import {Color} from "./color";
import {SetType} from "./set-type";
import inquirer from "inquirer";

type MenuChoiceValue = "edit-sets" | "end-turn";

interface MenuChoice {
  name: string,
  value: MenuChoiceValue,
}

function listToString<T>(list: T[]): string {
  if (list.length === 0) {
    return "<None>";
  }

  return list.map((value, index) => `[${index}]: ${value}`).join("\n");
}

// TODO: Could consider having option to play with playing cards instead of tiles (suits instead of colours,
//  card ranks instead of numbers, etc.)
// TODO: Could provide messages for invalid sets - e.g. "Set is too short", "Run must contain tiles of the same colour"
// TODO: Check player has actually placed any of their tiles after editing sets (maybe change menu)
// TODO: Maybe add titles to each section of the program
// TODO: Maybe prevent colours from repeating in groups, perhaps as an option
// TODO: Potentially add ability to detect if any moves are possible
// TODO: Could highlight the newly moved tile
// TODO: Allow moving a tile back to player tiles
// TODO: Allow not saving updated sets even if valid
// TODO: Hide <My tiles> from source list options if empty

export class Game {
  players: Player[] = [];
  tiles: Tile[] = [];
  sets: Set[] = [];

  async play(): Promise<void> {
    await this.setup();

    let currentPlayerIndex = 0;
    let consecutivePasses = 0;
    while (consecutivePasses < this.players.length && this.players.every(player => player.tiles.length > 0)) {
      let currentPlayer = this.players[currentPlayerIndex];

      console.log(`${currentPlayer.name}'s turn.`);

      // Print sets and player's tiles
      console.log("Sets:");
      console.log(listToString(this.sets));
      console.log("Your tiles:");
      console.log(listToString(currentPlayer.tiles));
      console.log();

      let valid = false;
      let menuChoice: MenuChoiceValue;
      do {
        const choices: MenuChoice[] = [
          {
            name: "Edit sets",
            value: "edit-sets",
          },
          {
            name: valid ? "End turn" : (this.tiles.length === 0 ? "Pass" : "Pass and take tile"),
            value: "end-turn",
          },
        ];
        menuChoice = (await inquirer.prompt([
          {
            type: "list",
            name: "menuChoice",
            message: "Choose an option",
            choices,
          },
        ])).menuChoice;

        switch (menuChoice) {
          case "edit-sets":
            valid = await this.editSets(currentPlayer);
            break;
          case "end-turn":
            if (this.tiles.length > 0 && !valid) {
              const newTile = this.tiles[0];
              this.tiles = this.tiles.slice(1);
              currentPlayer.tiles.push(newTile);
              console.log(`Picked up: ${newTile}`);
              console.log();
            }
            break;
        }
      } while (menuChoice !== "end-turn");

      if (menuChoice === "end-turn" && this.tiles.length === 0) {
        consecutivePasses++;
      } else {
        consecutivePasses = 0;
      }

      currentPlayerIndex = (currentPlayerIndex + 1) % this.players.length;
    }

    // Winning player is the one with fewest tiles remaining
    // If the game ended due to a player using up all their tiles (2nd condition above), the winner is that player
    const winningPlayer = this.players.reduce<Player | null>((acc, player) => acc === null || player.tiles.length < acc.tiles.length ? player : acc, null);
    if (winningPlayer !== null) {
      console.log(`${winningPlayer} wins!`);
    }
  }

  async setup(): Promise<void> {
    const playerNames: string[] = await this.inputPlayerNames();
    this.players = playerNames.map(playerName => new Player(playerName));

    this.generateTiles();
    this.distributeTiles();
  }

  async inputPlayerNames(): Promise<string[]> {
    let playerNames: string[] = [];

    let finished: boolean;
    do {
      let playerName: string;

      const defaultPlayerName = `Player ${(playerNames.length + 1)}`;
      playerName = (await inquirer.prompt([
        {
          type: "input",
          name: "playerName",
          message: `Player ${(playerNames.length + 1)}'s name`,
          default: defaultPlayerName,
          validate(playerName: string): string | true {
            return playerNames.includes(playerName) ? "Player name already exists. Please enter a different name." : true;
          }
        },
      ])).playerName;

      playerNames = [...playerNames, playerName];
      console.log(`Player "${playerName}" added.`);

      finished = !(await inquirer.prompt([
        {
          type: "confirm",
          name: "finished",
          message: "Add another player?",
          default: true,
        }
      ])).finished;
    } while (!finished);

    return playerNames;
  }

  generateTiles(): void {
    const tileMinRank = 1; // Inclusive
    const tileMaxRank = 14; // Exclusive
    const tileCopyCount = 2; // Number of copies of each tile to use

    const numberTiles = cartesianProduct(
      R.range(0, tileCopyCount),
      R.range(0, getEnumLength(Color)),
      R.range(tileMinRank, tileMaxRank),
    ).map(([, color, rank]) => new Tile(color, rank));

    // Generate smiley tiles
    const smileyTiles: Tile[] = [
      new Tile(Color.Black,null),
      new Tile(Color.Orange, null),
    ];

    this.tiles = [...numberTiles, ...smileyTiles];

    // Shuffle tiles
    // This way, can just get an element from the start or end, instead of having to pick a random element
    this.tiles = shuffle(this.tiles);
  }

  distributeTiles(): void {
    const playerTileCount = 14; // Number of tiles to initially give each player
    for (const player of this.players) {
      player.tiles = this.tiles.splice(0, playerTileCount);
    }
  }

  async editSets(player: Player): Promise<boolean> {
    const updatedPlayer = R.clone(player);
    let updatedSets = R.clone(this.sets);

    let finished: boolean;
    let valid: boolean;
    do {
      // Choose source list (list to move tile from)
      const sourceTileList = await Game.chooseSourceTileList(updatedSets, updatedPlayer);

      // Choose tile to remove from source list
      // Remove the tile
      // todo: check whether this works if array is empty
      const tileRemoveIndex: number = (await inquirer.prompt([
        {
          type: "list",
          name: "tileRemoveIndex",
          message: "Choose which tile to move",
          choices: sourceTileList.map((tile, index) => ({
            name: tile.toString(),
            value: index,
          })),
          loop: false,
        }
      ])).tileRemoveIndex;
      const tile = sourceTileList.splice(tileRemoveIndex, 1)[0];
      console.log();

      console.log("Moving the following tile:");
      console.log(tile.toString());
      console.log();

      // Remove empty sets
      updatedSets = updatedSets.filter(set => set.tiles.length > 0);

      // Choose destination list (list to move tile to)
      const destinationTileList = await Game.chooseDestinationTileList(updatedSets);

      // Choose index in destination list to insert tile at
      // Insert the tile
      const tileInsertIndex: number = (await inquirer.prompt([
        {
          type: "list",
          name: "tileInsertIndex",
          message: "Choose position to insert the tile at",
          choices: [...destinationTileList.map(tile => tile.toString()), "<End>"].map((choice, index) => ({
            name: choice,
            value: index,
          })),
          loop: false,
          default: destinationTileList.length,
        }
      ])).tileInsertIndex;
      destinationTileList.splice(tileInsertIndex, 0, tile);
      console.log();

      // Print updated sets and player's tiles
      console.log("Updated sets:");
      console.log(listToString(updatedSets));
      console.log("Your updated tiles:");
      console.log(listToString(updatedPlayer.tiles));
      console.log();

      // Check that all sets are valid and the player actually placed one or more tiles
      const setsValid = updatedSets.every(set => set.isValid());
      const playerTilesPlaced = updatedPlayer.tiles.length < player.tiles.length;
      valid = setsValid && playerTilesPlaced;

      if (valid) {
        console.log("Sets are currently VALID");
        console.log("Your changes will be SAVED if you choose not to continue editing sets");
      } else {
        if (setsValid) {
          console.log("Sets are currently VALID but you didn't place any of your tiles");
        } else {
          console.log("Sets are currently INVALID");
        }
        console.log("Your changes will be LOST if you choose not to continue editing sets");
      }

      finished = !(await inquirer.prompt([
        {
          type: "confirm",
          name: "finished",
          message: "Continue editing sets?",
          default: true,
        }
      ])).finished;
    } while (!finished);

    if (valid) {
      player.tiles = updatedPlayer.tiles;
      this.sets = updatedSets;
    }

    return valid;
  }

  static async chooseSourceTileList(sets: Set[], player: Player): Promise<Tile[]> {
    const choice: number | null = (await inquirer.prompt([
      {
        type: "list",
        name: "choice",
        message: "Choose where to move a tile from",
        choices: [
          ...sets.map((set, index) => ({
            name: set.toString(),
            value: index,
          })),
          {
            name: "<My tiles>",
            value: null,
          },
        ],
        default: sets.length,
      }
    ])).choice;

    if (choice === null) {
      return player.tiles;
    }

    return sets[choice].tiles;
  }

  static async chooseDestinationTileList(sets: Set[]): Promise<Tile[]> {
    const choice: number | null = (await inquirer.prompt([
      {
        type: "list",
        name: "choice",
        message: "Choose where to move the tile to",
        choices: [
          ...sets.map((set, index) => ({
            name: set.toString(),
            value: index,
          })),
          {
            name: "<New set>",
            value: null,
          },
        ],
        default: sets.length,
      }
    ])).choice;

    if (choice === null) {
      const newSet = new Set();
      sets.push(newSet);
      return newSet.tiles;
    }

    return sets[choice].tiles;
  }
}
