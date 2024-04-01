import React, { createContext, ReactNode, useContext, useEffect, useState } from "react"
import { DiscordSDK, DiscordSDKMock, IDiscordSDK } from "@discord/embedded-app-sdk"

const EmbeddedAppSdkContext = createContext<{
  sdk: IDiscordSDK | null
  mocked: boolean
}>({ sdk: null, mocked: false })
export const useEmbeddedAppSdk = () => useContext(EmbeddedAppSdkContext)

export function EmbeddedAppSdkProvider(props: { children: ReactNode }) {
  const [privateSdk, setPrivateSdk] = useState<IDiscordSDK | null>(null)
  const [mocked, setMocked] = useState(false)
  const [publicSdk, setPublicSdk] = useState<IDiscordSDK | null>(null)

  // Create SDK
  useEffect(() => {
    if (!process.env.PUBLIC_ENV__DISCORD_CLIENT_ID) return

    const sdk = new DiscordSDK(process.env.PUBLIC_ENV__DISCORD_CLIENT_ID)
    setMocked(false) // TODO: Setup mocked SDK
    setPrivateSdk(sdk)

    return () => setPrivateSdk(null)
  }, [process.env.PUBLIC_ENV__DISCORD_CLIENT_ID])

  // Set SDK when ready
  useEffect(() => {
    if (!privateSdk) return

    privateSdk.ready().then(() => setPublicSdk(privateSdk))

    return () => setPublicSdk(null)
  }, [privateSdk])

  // Render provider
  return (
    <EmbeddedAppSdkContext.Provider value={{ sdk: publicSdk, mocked }}>{props.children}</EmbeddedAppSdkContext.Provider>
  )
}
