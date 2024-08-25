import { WebPubSubClient } from "@azure/web-pubsub-client"
import React, { createContext, ReactNode, useContext, useEffect, useState } from "react"
import { useApi } from "./useApi"
import { useOrigin } from "./useOrigin"
import { useEmbeddedAppSdk } from "./useEmbeddAppSdk"

// Increase maximum listeners to allow multiple components to subscribe to websocket events
const maxListeners = 100
require("events").EventEmitter.prototype._maxListeners = maxListeners
require("events").defaultMaxListeners = maxListeners

type TWebsocketContext = {
  ws: WebPubSubClient | null
  connectionFailed: boolean
}

const WebsocketContext = createContext<TWebsocketContext>({ ws: null, connectionFailed: false })
export const useWebsocket = () => useContext(WebsocketContext)

export function WebsocketProvider(props: { children: ReactNode }) {
  const { origin } = useOrigin()
  const { api, hasToken } = useApi()
  const { user } = useEmbeddedAppSdk()

  const [client, setClient] = useState<WebPubSubClient | null>(null)
  const [ws, setWs] = useState<WebPubSubClient | null>(null)
  const [connectionFailed, setConnectionFailed] = useState(false)

  useEffect(() => {
    if (!user || !origin || !hasToken) return

    api
      .negotiate(user.id)
      .then(([res]) =>
        origin === "browser"
          ? res.url
          : `wss://${process.env.PUBLIC_ENV__DISCORD_CLIENT_ID}.discordsays.com/ws/client/hubs/Game?access_token=${res.accessToken}`
      )
      .then(url => setClient(new WebPubSubClient({ getClientAccessUrl: url })))
      .catch(() => setConnectionFailed(true))

    return () => setClient(null)
  }, [user, origin, hasToken])

  useEffect(() => {
    if (!client) return

    client
      .start()
      .then(() => setWs(client))
      .catch(() => setConnectionFailed(true))

    return () => {
      client.stop()
      setWs(null)
      setConnectionFailed(false)
    }
  }, [client])

  return <WebsocketContext.Provider value={{ ws, connectionFailed }}>{props.children}</WebsocketContext.Provider>
}
