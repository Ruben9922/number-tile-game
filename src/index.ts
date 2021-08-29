import {getEnumLength} from "./utilities";
import * as R from "remeda";
import {cartesianProduct} from "cartesian-product-multiple-arrays";
import shuffle from "shuffle-array";
import Player from "./player";
import Tile from "./tile";
import Set from "./set";
import {Color} from "./color";
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

function tileCompareFunction(tile1: Tile, tile2: Tile) {
  // Sort player tiles first by rank and then by colour
  if (tile1.rank === tile2.rank) {
    return tile1.color - tile2.color;
  }
  if (tile1.rank === null) {
    return -1;
  }
  if (tile2.rank === null) {
    return 1;
  }
  return tile1.rank - tile2.rank;
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

async function play(): Promise<void> {
  await setup();

  let currentPlayerIndex = 0;
  let consecutivePasses = 0;
  while (consecutivePasses < players.length && players.every(player => player.tiles.length > 0)) {
    let currentPlayer = players[currentPlayerIndex];

    console.log(`${currentPlayer.name}'s turn.`);

    // Print sets and player's tiles
    console.log("Sets:");
    console.log(listToString(sets));
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
          name: valid ? "End turn" : (tiles.length === 0 ? "Pass" : "Pass and take tile"),
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
          valid = await editSets(currentPlayer);
          break;
        case "end-turn":
          if (tiles.length > 0 && !valid) {
            const newTile = tiles[0];
            tiles = tiles.slice(1);
            currentPlayer.tiles.push(newTile);
            console.log(`Picked up: ${newTile}`);
            console.log();
            currentPlayer.tiles.sort(tileCompareFunction);
          }
          break;
      }
    } while (menuChoice !== "end-turn");

    if (menuChoice === "end-turn" && tiles.length === 0) {
      consecutivePasses++;
    } else {
      consecutivePasses = 0;
    }

    currentPlayerIndex = (currentPlayerIndex + 1) % players.length;
  }

  // Winning player is the one with fewest tiles remaining
  // If the game ended due to a player using up all their tiles (2nd condition above), the winner is that player
  const winningPlayer = players.reduce<Player | null>((acc, player) => acc === null || player.tiles.length < acc.tiles.length ? player : acc, null);
  if (winningPlayer !== null) {
    console.log(`${winningPlayer} wins!`);
  }
}

async function setup(): Promise<void> {
  const playerNames: string[] = await inputPlayerNames();
  players = playerNames.map(playerName => new Player(playerName));

  generateTiles();
  distributeTiles();
}

async function inputPlayerNames(): Promise<string[]> {
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

function generateTiles(): void {
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

  tiles = [...numberTiles, ...smileyTiles];

  // Shuffle tiles
  // This way, can just get an element from the start or end, instead of having to pick a random element
  tiles = shuffle(tiles);
}

function distributeTiles(): void {
  const playerTileCount = 14; // Number of tiles to initially give each player
  for (const player of players) {
    player.tiles = tiles.splice(0, playerTileCount);
    player.tiles.sort(tileCompareFunction);
  }
}

async function editSets(player: Player): Promise<boolean> {
  const updatedPlayer = R.clone(player);
  let updatedSets = R.clone(sets);

  let finished: boolean;
  let valid: boolean;
  do {
    // Choose source list (list to move tile from)
    const sourceTileList = await chooseSourceTileList(updatedSets, updatedPlayer);

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
    const destinationTileList = await chooseDestinationTileList(updatedSets);

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
    sets = updatedSets;
  }

  return valid;
}

async function chooseSourceTileList(sets: Set[], player: Player): Promise<Tile[]> {
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

async function chooseDestinationTileList(sets: Set[]): Promise<Tile[]> {
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

let players: Player[] = [];
let tiles: Tile[] = [];
let sets: Set[] = [];

play();
