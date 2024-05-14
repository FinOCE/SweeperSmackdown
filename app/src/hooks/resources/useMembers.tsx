import React, { createContext, useContext, useState } from "react"
import { useApi } from "../useApi"
import { useWebsocket } from "../useWebsocket"

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

  const [members, setMembers] = useState<TMember[] | null>(null)

  return <MemberContext.Provider value={{ members }}>{props.children}</MemberContext.Provider>
}
