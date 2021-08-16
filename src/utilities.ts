export function getEnumLength(e: { [id: number]: string }): number {
  return Object.keys(e).filter(x => !isNaN(Number(x))).length;
}
