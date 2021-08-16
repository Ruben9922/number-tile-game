// import {Player} from "./player";
// import {Tile} from "./tile";
// import {Set} from "./set";
// import * as R from "ramda";
// import readlineSync from "readline-sync";
// import {Color} from "./color";
// import {getEnumLength} from "./utilities";
// import arrayShuffle from "array-shuffle";
//
// class Game {
//   players: Player[] = [];
//   tiles: Tile[] = [];
//   sets: Set[] = [];
//
//   play(): void {
//     this.setup();
//
//     let currentPlayerIndex = 0;
//     let consecutivePasses = 0;
//     while (consecutivePasses < R.length(this.players) && R.all(player => !R.isEmpty(player.tiles), this.players)) {
//       let currentPlayer = this.players[currentPlayerIndex];
//
//       console.log(`${currentPlayer.name}'s turn.`);
//
//       // Print sets and player's tiles
//       console.log("Sets:");
//       console.log(this.sets); // todo
//       console.log("Your tiles:");
//       console.log(currentPlayer.tiles); // todo
//       console.log();
//
//       let valid = false;
//       let option: number;
//       do {
//         option = readlineSync.keyInSelect([
//           "Edit sets",
//           valid ? "End turn" : (R.isEmpty(this.tiles) ? "Pass" : "Pass and take tile"),
//         ], "Choose an option");
//         console.log();
//
//         switch (option) {
//           case 0:
//             valid = this.editSets(currentPlayer);
//             break;
//           case 1:
//             if (!R.isEmpty(this.tiles) && !valid) {
//               const newTile = R.head(this.tiles)!;
//               this.tiles = R.tail(this.tiles);
//               currentPlayer.tiles = R.append(newTile, currentPlayer.tiles);
//               console.log(`Picked up: ${newTile}`);
//               console.log();
//             }
//             break;
//         }
//       } while (option !== 1);
//
//       if (option === 1 && R.isEmpty(this.tiles)) {
//         consecutivePasses++;
//       } else {
//         consecutivePasses = 0;
//       }
//
//       currentPlayerIndex = (currentPlayerIndex + 1) % R.length(this.players);
//       // TODO: Compute winning player
//     }
//
//     // Winning player is the one with fewest tiles remaining
//     // If the game ended due to a player using up all their tiles (2nd condition above), the winner is that player
//     const winningPlayer = R.reduce<Player, Player | null>((acc, player) => acc === null || R.length(player.tiles) < R.length(acc.tiles) ? player : acc, null, this.players);
//     if (winningPlayer !== null) {
//       console.log(`${winningPlayer} wins!`);
//     }
//   }
//
//   setup(): void {
//     const playerNames: string[] = this.inputPlayerNames();
//     this.players = R.map(pn => new Player(pn), playerNames);
//
//     this.generateTiles();
//     this.distributeTiles();
//   }
//
//   inputPlayerNames(): string[] {
//     let playerNames: string[] = [];
//
//     let finished: boolean;
//     do {
//       let valid: boolean;
//       let playerName: string;
//
//       do {
//         const defaultPlayerName = `Player ${(R.length(playerNames) + 1)}`;
//         playerName = readlineSync.question(`Enter Player ${(R.length(playerNames) + 1)}'s name (leave blank for "${defaultPlayerName}"`);
//         playerName = R.trim(playerName);
//
//         if (R.isEmpty(playerName)) {
//           playerName = defaultPlayerName;
//         }
//
//         if (R.includes(playerName, playerNames)) {
//           console.log("Player name already exists. Please enter a different name.");
//           valid = false;
//         } else {
//           valid = true;
//         }
//       } while (!valid);
//
//       playerNames = R.append(playerName, playerNames);
//       console.log(`Player "${playerName}" added.`);
//
//       finished = !readlineSync.keyInYN("Add another player? ");
//       console.log();
//     } while (!finished);
//
//     return playerNames;
//   }
//
//   generateTiles(): void {
//     // Generate number tiles
//     const numberTiles: Tile[] = R.map<[number, [Color, number]], Tile>(
//       ([_, [color, rank]]) => new Tile(color, rank),
//       R.xprod(R.range(0, Tile.copyCount), R.xprod(R.range(0, getEnumLength(Color)), R.range(Tile.minRank, Tile.maxRank)))
//     );
//
//     // Generate smiley tiles
//     const smileyTiles: Tile[] = [
//       new Tile(Color.Black, null),
//       new Tile(Color.Orange, null),
//     ]
//
//     this.tiles = R.concat(numberTiles, smileyTiles);
//
//     // Shuffle tiles
//     // This way, can just get an element from the start or end, instead of having to pick a random element
//     this.tiles = arrayShuffle(this.tiles);
//   }
//
//   distributeTiles(): void {
//     const splitTiles = R.splitEvery(Player.tileCount, this.tiles);
//     this.players = R.zipWith((player, tiles) => new Player(player.name, tiles), this.players, splitTiles);
//     this.tiles = R.slice(R.length(this.players) * Player.tileCount, R.length(this.tiles), this.tiles);
//   }
//
//   editSets(player: Player): boolean {
//     const updatedPlayerTiles = R.clone(player.tiles);
//     const updatedSets = R.clone(this.sets);
//
//     const option: number;
//     do {
//       // Choose list to move tile from (source list)
//       const sourceTileList = Game.chooseSourceTileList(updatedSets, updatedPlayerTiles);
//
//       // Choose tile to remove from source list and remove the tile
//       const tileIndex = readlineSync.keyInSelect(sourceTileList, "Choose which tile to move");
//       const tile = sou
//     } while (option);
//   }
//
//   chooseSourceTileList(sets: Set[], playerTiles: Tile[]): Tile[] {
//     const option = readlineSync.keyInSelect(R.append("<My tiles>", R.map(set => set.toString(), sets)), "Choose where to move a tile from");
//     console.log();
//
//     if (option === R.length(sets)) {
//       return playerTiles;
//     }
//
//     return sets[option].tiles;
//   }
//
//   chooseDestinationTileList(sets: Set[]): Tile[] {
//     const option = readlineSync.keyInSelect(R.append("<New set>", R.map(set => set.toString(), sets)), "Choose where to move the tile to");
//     console.log();
//
//     if (option === R.length(sets)) {
//       const newSet = new Set();
//       sets.push(newSet); // Not sure if there's a better way to do this!
//       return newSet.tiles;
//     }
//
//     return sets[option].tiles;
//   }
// }
