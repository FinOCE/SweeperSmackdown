import React, { createContext, ReactNode, useContext, useEffect, useState } from "react"
import { DiscordSDK, DiscordSDKMock, IDiscordSDK } from "@discord/embedded-app-sdk"

type TEmbeddedAppSdkContext = {
  sdk: IDiscordSDK | null
  mocked: boolean
}

const EmbeddedAppSdkContext = createContext<TEmbeddedAppSdkContext>({ sdk: null, mocked: false })
export const useEmbeddedAppSdk = () => useContext(EmbeddedAppSdkContext)

export function EmbeddedAppSdkProvider(props: { iframeId: string | null; children: ReactNode }) {
  const mocked = props.iframeId === null

  const [sdk, setSdk] = useState<IDiscordSDK | null>(null)

  // Create SDK
  useEffect(() => {
    if (!process.env.PUBLIC_ENV__DISCORD_CLIENT_ID) return

    const sdk: IDiscordSDK = mocked
      ? getMockSdk(process.env.PUBLIC_ENV__DISCORD_CLIENT_ID)
      : new DiscordSDK(process.env.PUBLIC_ENV__DISCORD_CLIENT_ID)

    sdk.ready().then(() => setSdk(sdk))

    return () => setSdk(null)
  }, [process.env.PUBLIC_ENV__DISCORD_CLIENT_ID])

  // Render provider
  return <EmbeddedAppSdkContext.Provider value={{ sdk, mocked }}>{props.children}</EmbeddedAppSdkContext.Provider>
}

function getMockSdk(clientId: string) {
  const mock = new DiscordSDKMock(clientId, null, null)

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
          discriminator: "",
          id: args.access_token ?? "",
          public_flags: 0,
          username: ""
        }
      }
    }
  })

  return mock
}
