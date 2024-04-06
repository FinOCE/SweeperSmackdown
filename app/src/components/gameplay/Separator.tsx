import React from "react"
import "./Separator.scss"
import { BoardStylingUtil } from "../../utils/BoardStylingUtil"

type IntersectionProps = {
  orientation: "intersection"
  topLeft: number
  topRight: number
  bottomLeft: number
  bottomRight: number
}

type HorizontalProps = {
  orientation: "horizontal"
  top: number
  bottom: number
}

type VerticalProps = {
  orientation: "vertical"
  left: number
  right: number
}

export type SeparatorProps = IntersectionProps | HorizontalProps | VerticalProps

export function Separator(props: SeparatorProps) {
  function isIntersection(props: SeparatorProps): props is IntersectionProps {
    return props.orientation === "intersection"
  }

  function isHorizontal(props: SeparatorProps): props is HorizontalProps {
    return props.orientation === "horizontal"
  }

  function isVertical(props: SeparatorProps): props is VerticalProps {
    return props.orientation === "vertical"
  }

  if (isIntersection(props)) {
    const hide =
      [
        BoardStylingUtil.getType(props.topLeft),
        BoardStylingUtil.getType(props.topRight),
        BoardStylingUtil.getType(props.bottomLeft),
        BoardStylingUtil.getType(props.bottomRight)
      ].reduce((pre, cur) => (pre === cur ? pre : "invalid")) !== "invalid"
        ? true
        : false

    return (
      <div className="separator-intersection">
        <div className="separator-intersection-circle" style={{ display: hide ? "none" : "block" }} />
      </div>
    )
  } else if (isHorizontal(props)) {
    const isBridging = BoardStylingUtil.isConnectedHorizontally(props.top, props.bottom)
    const type = BoardStylingUtil.getType(props.top)

    return (
      <div className="separator-horizontal">
        <div className={`separator-horizontal-line separator-line-${isBridging ? type : "different"}`} />
      </div>
    )
  } else if (isVertical(props)) {
    const isBridging = BoardStylingUtil.isConnectedVertically(props.left, props.right)
    const type = BoardStylingUtil.getType(props.left)

    return (
      <div className="separator-vertical">
        <div className={`separator-vertical-line separator-line-${isBridging ? type : "different"}`} />
      </div>
    )
  } else throw new Error("Invalid separator orientation provided")
}
