import Tile from "./tile";

export default class Player {
  readonly name: string;
  tiles: Tile[] = [];

  constructor(name: string) {
    this.name = name;
  }
}
