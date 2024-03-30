export class State {
  public static mask(startIndex: number, endIndex: number = startIndex): number {
    let value = 0

    for (let i = startIndex; i <= endIndex; i++) value += 1 << i

    return value
  }

  public static containsBit(state: number, offset: number): boolean {
    return (this.mask(offset) & state) === this.mask(offset)
  }

  public static reveal(state: number): number {
    return (1 << 0) | state
  }

  public static isRevealed(state: number): boolean {
    return this.containsBit(state, 0)
  }

  public static flag(state: number): number {
    return (1 << 1) | state
  }

  public static removeFlag(state: number): number {
    return (this.mask(0) | this.mask(2, 7)) & state
  }

  public static isFlagged(state: number): boolean {
    return this.containsBit(state, 1)
  }

  public static getAdjacentBombCount(state: number): number {
    return (this.mask(2, 5) & state) >> 2
  }

  public static isBomb(state: number): boolean {
    return this.getAdjacentBombCount(state) === 9
  }

  public static isEmpty(state: number): boolean {
    return this.getAdjacentBombCount(state) === 0
  }

  public static matchVariableBits(state: number, bit1: boolean, bit2: boolean): boolean {
    return bit1 ? this.containsBit(state, 6) : true && bit2 ? this.containsBit(state, 7) : true
  }

  public static isRevealedEquivalent(oldState: number, newState: number): boolean {
    return (oldState | this.mask(0)) === newState
  }

  public static isRevealEquivalentAll(initialState: number[], gameState: number[]): boolean {
    if (initialState.length !== gameState.length) return false

    for (let i = 0; i < initialState.length; i++)
      if (!this.isRevealedEquivalent(initialState[i], gameState[1])) return false

    return true
  }

  public static create(
    isRevealed: boolean = false,
    isFlagged: boolean = false,
    isBomb: boolean = false,
    adjacentBombs: number = 0,
    bit1: boolean = false,
    bit2: boolean = false
  ): number {
    if (adjacentBombs > 8 || adjacentBombs < 0) throw new Error("There can only be between 0 and 8 adjacent bombs")

    let state = 0

    if (isRevealed) state += 1 << 0
    if (isFlagged) state += 1 << 1
    state += (isBomb ? 9 : adjacentBombs) << 2
    if (bit1) state += 1 << 6
    if (bit2) state += 1 << 7

    return state
  }
}
