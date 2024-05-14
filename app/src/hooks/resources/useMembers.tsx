import React, { createContext, useContext, useEffect, useState } from "react"
import { useLobbyData } from "../data/useLobbyData"
import { useEmbeddedAppSdk } from "../useEmbeddAppSdk"
import { getAvatarUrl, getUsername } from "../../utils/getDisplayDetails"

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
  const { participants, user } = useEmbeddedAppSdk()
  const { lobbyData } = useLobbyData()

  const [members, setMembers] = useState<TMember[] | null>(null)

  useEffect(() => {
    if (!lobbyData) {
      setMembers(null)
      return
    }

    const members = lobbyData.userIds.map(id => {
      const displayName = getUsername(id, user, participants ?? []) ?? id
      const avatarUrl = getAvatarUrl(id, user, participants ?? [])
      return { id, displayName, avatarUrl } as TMember
    })

    setMembers(members)
  }, [lobbyData, participants, user])

  return <MemberContext.Provider value={{ members }}>{props.children}</MemberContext.Provider>
}
