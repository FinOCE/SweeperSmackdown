import React, { createContext, ReactNode, useContext, useEffect, useState } from "react"
import { useEmbeddedAppSdk } from "./useEmbeddAppSdk"
import { useApi } from "./useApi"
import { IDiscordSDK } from "@discord/embedded-app-sdk"
import { User } from "../types/User"

const UserContext = createContext<User | null>(null)
export const useUser = () => useContext(UserContext)

export function UserProvider(props: { children: ReactNode }) {
  const { api, setToken } = useApi()
  const { sdk, mocked } = useEmbeddedAppSdk()

  const [user, setUser] = useState<User | null>(null)

  useEffect(() => {
    if (sdk) login(sdk, mocked)
  }, [sdk])

  async function login(sdk: IDiscordSDK, mocked: boolean) {
    // Get oauth code from sdk
    const { code } = await sdk.commands.authorize({
      client_id: process.env.PUBLIC_ENV__DISCORD_CLIENT_ID,
      response_type: "code",
      scope: ["identify"]
    })

    // Exchange code for access token
    const { accessToken } = await api.token(code, mocked)

    // Exchange access token for bearer token for API
    const { bearerToken } = await api.login(accessToken, mocked)

    // Get user info from sdk using access token
    const auth = await sdk.commands.authenticate({ access_token: accessToken })

    // Set user context
    setToken(bearerToken)
    setUser({
      id: auth.user.id,
      onDiscord: mocked
    })
  }

  return <UserContext.Provider value={user}>{props.children}</UserContext.Provider>
}
