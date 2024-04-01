import { WebPubSubClient } from "@azure/web-pubsub-client"
import React, { createContext, ReactNode, useContext, useEffect, useState } from "react"
import { useApi } from "./useApi"
import { EventManager, IEventManager } from "../managers/EventManager"
import { useUser } from "./useUser"

const WebsocketContext = createContext<IEventManager | null>(null)
export const useWebsocket = () => useContext(WebsocketContext)

export function WebsocketProvider(props: { children: ReactNode }) {
  const { api } = useApi()
  const user = useUser()

  const [ready, setReady] = useState(false)

  const [client, setClient] = useState<WebPubSubClient | null>(null)
  const [manager, setManager] = useState<EventManager | null>(null)

  useEffect(() => {
    if (!user) return

    setClient(
      new WebPubSubClient({
        getClientAccessUrl: () => api.negotiate(user.id).then(res => res.url)
      })
    )

    return () => setClient(null)
  }, [user])

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

    return () => setManager(null)
  }, [ready, client])

  return <WebsocketContext.Provider value={manager}>{props.children}</WebsocketContext.Provider>
}
