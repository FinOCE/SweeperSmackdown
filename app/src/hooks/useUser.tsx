import React, { createContext, ReactNode, useContext, useEffect, useState } from "react"
import { useEmbeddedAppSdk } from "./useEmbeddAppSdk"
import { getDiscordAvatarUrl } from "../utils/getDiscordAvatarUrl"
import { useApi } from "./useApi"

export type User = {
  id: string
  username: string
  avatarUrl: string | null
  onDiscord: boolean
}

const UserContext = createContext<User | null>(null)
export const useUser = () => useContext(UserContext)

export function UserProvider(props: { children: ReactNode }) {
  const { api, setToken } = useApi()
  const { sdk, mocked } = useEmbeddedAppSdk()

  const [user, setUser] = useState<User | null>(null)

  useEffect(() => {
    if (!sdk) return

    sdk.commands
      .authorize({
        client_id: process.env.PUBLIC_ENV__DISCORD_CLIENT_ID,
        response_type: "code",
        scope: ["identify"]
      })
      .then(({ code }) => api.token(code, mocked))
      .then(({ access_token }) => sdk.commands.authenticate({ access_token }))
      .then(auth => {
        setToken(auth.access_token)
        setUser({
          id: auth.user.id,
          username: auth.user.username,
          avatarUrl: mocked ? null : getDiscordAvatarUrl(auth.user.id, auth.user.avatar),
          onDiscord: mocked
        })
      })
  }, [sdk])

  return <UserContext.Provider value={user}>{props.children}</UserContext.Provider>
}
