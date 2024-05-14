import React, { createContext, useContext, useEffect, useState } from "react"
import { useApi } from "../useApi"
import { useWebsocket } from "../useWebsocket"
import { useLobby } from "./useLobby"

type TMember = {
  id: string
  displayName: string
  avatarUrl: string | null
}

type TMemberContext = {
  members: TMember[] | null
}

const MemberContext = createContext<TMemberContext>({ members: null })
export const useMembers = () => useContext(MemberContext)

export function MemberProvider(props: { children?: React.ReactNode }) {
  const { api } = useApi()
  const { ws } = useWebsocket()
  const { lobby } = useLobby()

  const [members, setMembers] = useState<TMember[] | null>(null)

  useEffect(() => {
    if (!lobby) setMembers(null)
  }, [lobby])

  useEffect(() => {
    if (!ws) return

    // TODO: Handle websocket events

    return () => {}
  }, [ws])

  return <MemberContext.Provider value={{ members }}>{props.children}</MemberContext.Provider>
}
