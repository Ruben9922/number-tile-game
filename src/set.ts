import Tile from "./tile";
import {SetType} from "./set-type";

export default class Set {
  tiles: Tile[] = [];

  computeSetType(): [SetType, string] {
    const minSize = 3;

    // Check whether set is too short
    if (this.tiles.length < minSize) {
      return [SetType.Invalid, "Set is too short"];
    }

    // Obtain first tile that isn't a smiley
    const refTileIndex: number = this.tiles.findIndex(tile => tile.rank !== null);
    const refTile: Tile | null = refTileIndex === -1 ? null : this.tiles[refTileIndex];

    // If there is no tile that isn't a smiley, there are 3+ tiles that are all smileys, which is valid
    // Can consider this as either a run or group, as smileys' colours are ignored
    if (refTile === null) {
      return [SetType.Group, ""];
    }

    // Check if run
    // Specifically, check if ranks are consecutive and colours are the same (ignoring smileys)
    // const isRun: boolean = R.all(
    //   (x: boolean) => x,
    //   R.addIndex<Tile>(R.map)(
    //     (tile: Tile, index: number) =>
    //       tile.rank === null || (tile.rank === refTile.rank! - refTileIndex + index && tile.color === refTile.color),
    //     tiles
    //   )
    // );
    const isRun: boolean = this.tiles.every((tile, index) =>
      tile.rank === null || (tile.rank === refTile.rank! - refTileIndex + index && tile.color === refTile.color));
    if (isRun) {
      return [SetType.Run, ""];
    }

    const isGroup: boolean = this.tiles.every(tile => tile.rank === null || tile.rank === refTile.rank);
    if (isGroup) {
      return [SetType.Group, ""];
    }

    const looksLikeRun: boolean = this.tiles.every((tile, index) => tile.rank === null || tile.rank === refTile.rank! - refTileIndex + index);
    if (looksLikeRun) {
      return [SetType.Invalid, "Tiles in a run must be of the same colour"];
    }

    return [SetType.Invalid, ""];
  }

  isValid(): boolean {
    const [setType] = this.computeSetType();
    return setType !== SetType.Invalid;
  }

  toString(): string {
    const [setType, errorMessage] = this.computeSetType();
    return `${this.tiles.join(", ")} (${SetType[setType]}${errorMessage === "" ? "" : ` - ${errorMessage}`})`;
  }
}
