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
}

const WebsocketContext = createContext<TWebsocketContext>({ ws: null })
export const useWebsocket = () => useContext(WebsocketContext)

export function WebsocketProvider(props: { children: ReactNode }) {
  const { origin } = useOrigin()
  const { api, hasToken } = useApi()
  const { user } = useEmbeddedAppSdk()

  const [client, setClient] = useState<WebPubSubClient | null>(null)
  const [ws, setWs] = useState<WebPubSubClient | null>(null)
  const [retries, setRetries] = useState(0)

  useEffect(() => {
    if (!user || !hasToken) return

    if (retries > 3) {
      alert("Unable to connect to the game servers. Please try again later.")
      return
    }

    api
      .negotiate(user.id)
      .then(([res]) =>
        origin === "browser"
          ? res.url
          : `wss://${process.env.PUBLIC_ENV__DISCORD_CLIENT_ID}.discordsays.com/ws/client/hubs/Game?access_token=${res.accessToken}`
      )
      .then(url => {
        try {
          setClient(new WebPubSubClient({ getClientAccessUrl: url }))
        } catch (err) {
          console.log("Failed to connect to web pubsub but caught error:", err)
        }
      })
      .catch(() => setRetries(prev => prev + 1))

    return () => setClient(null)
  }, [user, origin, hasToken, retries])

  useEffect(() => {
    if (!client) return

    client.start().then(() => setWs(client))

    return () => {
      client.stop()
      setWs(null)
    }
  }, [client])

  return <WebsocketContext.Provider value={{ ws }}>{props.children}</WebsocketContext.Provider>
}
