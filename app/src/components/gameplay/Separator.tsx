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
    let clipPath = "polygon("

    if (BoardStylingUtil.isRoundedTopLeftCorner(props.bottomRight, props.topRight, props.bottomLeft, props.topLeft))
      clipPath += "100% 50%, 50% 50%, 50% 100%"
    else clipPath += "100% 50%, 100% 100%, 50% 100%"

    clipPath += ", "

    if (BoardStylingUtil.isRoundedTopRightCorner(props.bottomLeft, props.topLeft, props.bottomRight, props.topRight))
      clipPath += "50% 100%, 50% 50%, 0% 50%"
    else clipPath += "50% 100%, 0% 100%, 0% 50%"

    clipPath += ", "

    if (BoardStylingUtil.isRoundedBottomRightCorner(props.topLeft, props.bottomLeft, props.topRight, props.bottomRight))
      clipPath += "0% 50%, 50% 50%, 50% 0%"
    else clipPath += "0% 50%, 0% 0%, 50% 0%"

    clipPath += ", "

    if (BoardStylingUtil.isRoundedBottomLeftCorner(props.topRight, props.bottomRight, props.topLeft, props.bottomLeft))
      clipPath += "50% 0%, 50% 50%, 100% 50%"
    else clipPath += "50% 0%, 100% 0%, 100% 50%"

    clipPath += ")"

    return (
      <div className="separator-intersection">
        <div className="separator-intersection-circle" style={{ clipPath }} />
      </div>
    )
  } else if (isHorizontal(props)) {
    const isBridging = BoardStylingUtil.isConnectedHorizontally(props.top, props.bottom)

    return (
      <div className="separator-horizontal">
        <div className="separator-horizontal-line" style={{ display: isBridging ? "block" : "none" }} />
      </div>
    )
  } else if (isVertical(props)) {
    const isBridging = BoardStylingUtil.isConnectedVertically(props.left, props.right)

    return (
      <div className="separator-vertical">
        <div className="separator-vertical-line" style={{ display: isBridging ? "block" : "none" }} />
      </div>
    )
  } else throw new Error("Invalid separator orientation provided")
}