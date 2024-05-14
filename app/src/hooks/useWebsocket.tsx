import { WebPubSubClient } from "@azure/web-pubsub-client"
import React, { createContext, ReactNode, useContext, useEffect, useState } from "react"
import { useApi } from "./useApi"
import { EventManager, IEventManager } from "../managers/EventManager"
import { useOrigin } from "./useOrigin"
import { useEmbeddedAppSdk } from "./useEmbeddAppSdk"

type TWebsocketContext = {
  ws: WebPubSubClient | null
  /** @deprecated Migrate to ws, removed once no longer used */
  manager: IEventManager | null
}

const WebsocketContext = createContext<TWebsocketContext>({
  ws: null,
  manager: null
})
export const useWebsocket = () => useContext(WebsocketContext)

export function WebsocketProvider(props: { children: ReactNode }) {
  const { origin } = useOrigin()
  const { api, hasToken } = useApi()
  const { user } = useEmbeddedAppSdk()

  const [ready, setReady] = useState(false)

  const [client, setClient] = useState<WebPubSubClient | null>(null)
  const [manager, setManager] = useState<EventManager | null>(null)
  const [ws, setWs] = useState<WebPubSubClient | null>(null)

  useEffect(() => {
    if (!user || !hasToken) return

    api
      .negotiate(user.id)
      .then(([res]) =>
        origin === "browser"
          ? res.url
          : `wss://${process.env.PUBLIC_ENV__DISCORD_CLIENT_ID}.discordsays.com/ws/client/hubs/Game?access_token=${res.accessToken}`
      )
      .then(url => {
        setClient(new WebPubSubClient({ getClientAccessUrl: url }))
      })

    return () => setClient(null)
  }, [user, origin, hasToken])

  useEffect(() => {
    if (!client) return

    client.start().then(() => setReady(true))
    return () => client.stop()
  }, [client])

  useEffect(() => {
    if (!client) return

    const manager = new EventManager(client)

    client.on("connected", e => manager.emit("connected", e))
    client.on("disconnected", e => manager.emit("disconnected", e))
    client.on("stopped", e => manager.emit("stopped", e))
    client.on("rejoin-group-failed", e => manager.emit("rejoin-group-failed", e))
    client.on("group-message", e => manager.emit("group-message", e))
    client.on("server-message", e => manager.emit("server-message", e))

    setManager(manager)
    setWs(client)

    return () => {
      setManager(null)
      setWs(null)
    }
  }, [ready, client])

  return <WebsocketContext.Provider value={{ manager, ws }}>{props.children}</WebsocketContext.Provider>
}
