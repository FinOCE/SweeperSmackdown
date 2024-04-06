import { SeparatorProps } from "../components/gameplay/Separator"
import { State } from "./State"

export class BoardStylingUtil {
  public static getProps(
    xi: number,
    yi: number,
    topLeft: number,
    topRight: number,
    bottomLeft: number,
    bottomRight: number
  ): SeparatorProps {
    if (xi % 2 === 0 && yi % 2 === 0) return { orientation: "intersection", topLeft, topRight, bottomLeft, bottomRight }
    else if (yi % 2 === 0) return { orientation: "horizontal", top: topRight, bottom: bottomRight }
    else return { orientation: "vertical", left: bottomLeft, right: bottomRight }
  }

  public static getSeparatorOrientation(xi: number, yi: number) {
    if (xi % 2 === 0 && yi % 2 === 0) return "intersection"
    else if (yi % 2 === 0) return "horizontal"
    else return "vertical"
  }

  public static getType(state: number) {
    if (State.isFlagged(state)) return "flag"
    else if (!State.isRevealed(state)) return "unrevealed"
    else if (State.isBomb(state)) return "bomb"
    else return "revealed"
  }

  public static isRoundedTopLeftCorner(origin: number, top: number, left: number, topLeft: number) {
    const type = this.getType(origin)

    return this.getType(top) !== type && this.getType(left) !== type
  }

  public static isRoundedTopRightCorner(origin: number, top: number, right: number, topRight: number) {
    const type = this.getType(origin)

    return this.getType(top) !== type && this.getType(right) !== type
  }

  public static isRoundedBottomLeftCorner(origin: number, bottom: number, left: number, bottomLeft: number) {
    const type = this.getType(origin)

    return this.getType(bottom) !== type && this.getType(left) !== type
  }

  public static isRoundedBottomRightCorner(origin: number, bottom: number, right: number, bottomRight: number) {
    const type = this.getType(origin)

    return this.getType(bottom) !== type && this.getType(right) !== type
  }

  public static isConnectedHorizontally(top: number, bottom: number) {
    return this.getType(top) === this.getType(bottom)
  }

  public static isConnectedVertically(left: number, right: number) {
    return this.getType(left) === this.getType(right)
  }
}
