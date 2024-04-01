import React, { createContext, ReactNode, useContext, useEffect, useState } from "react"
import { useEmbeddedAppSdk } from "./useEmbeddAppSdk"
import { getDiscordAvatarUrl } from "../utils/getDiscordAvatarUrl"
import { useApi } from "./useApi"

export type User = {
  id: string
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
      // Get access token from Discord
      .authorize({
        client_id: process.env.PUBLIC_ENV__DISCORD_CLIENT_ID,
        response_type: "code",
        scope: ["identify"]
      })
      .then(({ code }) => api.token(code, mocked))
      // Exchange access token for bearer token for API
      .then(async ({ accessToken }) => {
        const res = await api.login(accessToken, mocked)
        setToken(res.bearerToken)
        return accessToken
      })
      // Get user info from Discord with the access token
      .then(accessToken => sdk.commands.authenticate({ access_token: accessToken }))
      .then(auth =>
        setUser({
          id: auth.user.id,
          onDiscord: mocked
        })
      )
  }, [sdk])

  return <UserContext.Provider value={user}>{props.children}</UserContext.Provider>
}
