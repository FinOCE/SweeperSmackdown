import React, { createContext, ReactNode, useContext, useEffect, useState } from "react"
import { DiscordSDK, DiscordSDKMock, IDiscordSDK, type Types, type CommandTypes } from "@discord/embedded-app-sdk"
import { useOrigin } from "./useOrigin"
import { useApi } from "./useApi"

type TEmbeddedAppSdkContext = {
  sdk: IDiscordSDK | null
  participants: Participant[] | null
  user: User | null
}

export type Participant = Awaited<ReturnType<CommandTypes["getInstanceConnectedParticipants"]>>["participants"][0]
export type User = Awaited<ReturnType<CommandTypes["authenticate"]>>["user"]

const EmbeddedAppSdkContext = createContext<TEmbeddedAppSdkContext>({ sdk: null, participants: null, user: null })
export const useEmbeddedAppSdk = () => useContext(EmbeddedAppSdkContext)

export function EmbeddedAppSdkProvider(props: { children: ReactNode }) {
  const { origin } = useOrigin()
  const { api, setToken } = useApi()

  const [sdk, setSdk] = useState<IDiscordSDK | null>(null)
  const [participants, setParticipants] = useState<Participant[]>([])
  const [user, setUser] = useState<User | null>(null)

  // Create SDK
  useEffect(() => {
    if (!process.env.PUBLIC_ENV__DISCORD_CLIENT_ID) return

    const sdk: IDiscordSDK =
      origin === "browser"
        ? getMockSdk(process.env.PUBLIC_ENV__DISCORD_CLIENT_ID)
        : new DiscordSDK(process.env.PUBLIC_ENV__DISCORD_CLIENT_ID)

    sdk.ready().then(async () => {
      // Handle user
      const { code } = await sdk.commands.authorize({
        client_id: process.env.PUBLIC_ENV__DISCORD_CLIENT_ID,
        response_type: "code",
        state: "",
        prompt: "none",
        scope: ["identify"]
      })

      const [{ accessToken }] = await api.token(code, origin === "browser")
      const [{ bearerToken }] = await api.login(accessToken, origin === "browser")
      const auth = await sdk.commands.authenticate({ access_token: accessToken })

      setToken(bearerToken)
      setUser(auth.user)

      // Handle participants
      const { participants } = await sdk.commands.getInstanceConnectedParticipants()
      setParticipants(participants)

      sdk.subscribe("ACTIVITY_INSTANCE_PARTICIPANTS_UPDATE", ({ participants }) => setParticipants(participants))

      // Set sdk
      setSdk(sdk)
    })

    return () => setSdk(null)
  }, [origin, process.env.PUBLIC_ENV__DISCORD_CLIENT_ID])

  // Render provider
  return (
    <EmbeddedAppSdkContext.Provider value={{ sdk, participants, user }}>
      {props.children}
    </EmbeddedAppSdkContext.Provider>
  )
}

function getMockSdk(clientId: string) {
  const mock = new DiscordSDKMock(clientId, null, null)

  // TODO: Implement oauth where possible and handle fetching rest from api

  mock._updateCommandMocks({
    async authorize() {
      return {
        code: ""
      }
    },
    async authenticate(args) {
      return {
        access_token: args.access_token ?? "",
        application: {
          description: "",
          id: "",
          name: ""
        },
        expires: "",
        scopes: ["identify"],
        user: {
          discriminator: "-1", // User -1 discriminator to indicate it is mocked
          id: args.access_token ?? "",
          public_flags: 0,
          username: args.access_token ?? ""
        }
      }
    },
    async getInstanceConnectedParticipants() {
      return { participants: [] }
    }
  })

  return mock
}
