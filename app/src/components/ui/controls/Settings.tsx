import React, { ReactNode, useEffect, useRef, useState } from "react"
import "./Settings.scss"
import { Box } from "../Box"
import { Hamburger } from "../icons/Hamburger"
import { useClickOutside } from "../../../hooks/useClickOutside"

export function Settings(props: { children?: ReactNode }) {
  const [visible, setVisible] = useState(false)

  const button = useRef<HTMLDivElement>(null)
  const popup = useRef<HTMLDivElement>(null)

  useClickOutside([button, popup], () => setVisible(false))

  return (
    <>
      <div className="settings-button" ref={button}>
        <Box onClick={() => setVisible(v => !v)} innerClass="settings-hamburger-container">
          <Hamburger size={40} />
        </Box>
      </div>
      {visible && (
        <div className="settings-popup" ref={popup}>
          <Box disabled ignoreColorOverwrite>
            {props.children}
          </Box>
        </div>
      )}
    </>
  )
}
