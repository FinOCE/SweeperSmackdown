import React, { ReactNode } from "react"
import "./RollingBackground.scss"
import { Bomb } from "./icons/Bomb"
import { Flag } from "./icons/Flag"

export function RollingBackground(props: { children?: ReactNode; fade?: boolean }) {
  return (
    <div className={`rolling-background ${props.fade ? "rolling-background-fade" : ""}`}>
      <div className="rolling-background-bg-container">
        <div className="rolling-background-bg">
          {Array.from({ length: 15 }).map((_, i) => (
            <RollingBackgroundRow key={i} offset={i}>
              <Bomb color="off-bg" />
              <Flag color="off-bg" />
              <Bomb color="off-bg" />
              <Flag color="off-bg" />
            </RollingBackgroundRow>
          ))}
        </div>
      </div>
      <div className="rolling-background-content">{props.children}</div>
    </div>
  )
}

function RollingBackgroundRow(props: { children?: ReactNode; offset: number }) {
  return (
    <div className="rolling-background-row" style={{ right: `${props.offset * 100}px` }}>
      <div className="rolling-background-row-half">{props.children}</div>
      <div className="rolling-background-row-half">{props.children}</div>
      <div className="rolling-background-row-half">{props.children}</div>
    </div>
  )
}
