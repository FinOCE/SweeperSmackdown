import React, { createContext, ReactNode, useContext, useEffect, useState } from "react"
import { Loading } from "../components/Loading"

type Origin = "browser" | "discord"

type TOriginContext = {
  origin: Origin
  frameId: string | null
}

const OriginContext = createContext<TOriginContext>({ origin: "browser", frameId: null })
export const useOrigin = () => useContext(OriginContext)

export function OriginProvider(props: { children?: ReactNode }) {
  const [origin, setOrigin] = useState<Origin | null>(null)

  // Check if the app is running in an iframe (on Discord)
  const [frameId, setFrameId] = useState<string | null>(null)

  useEffect(() => {
    if (window?.location?.search !== "" && (window?.location?.search || null) === null) return

    const params = new URLSearchParams(window.location.search)
    const frameId = params.get("frame_id")

    setOrigin(frameId === null ? "browser" : "discord")
    setFrameId(frameId)
  }, [window?.location?.search])

  if (!origin) return <Loading />

  // Render provider and subsequent children
  return <OriginContext.Provider value={{ origin, frameId }}>{props.children}</OriginContext.Provider>
}
