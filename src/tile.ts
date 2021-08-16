import {Color} from "./color";

export default class Tile {
  readonly color: Color;
  readonly rank: number | null;

  constructor(color: Color, rank: number | null) {
    this.color = color;
    this.rank = rank;
  }

  toString(): string {
    return `${Color[this.color]} ${this.rank === null ? "Smiley" : this.rank}`;
  }
}
