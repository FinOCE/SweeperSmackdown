import {
  OnConnectedArgs,
  OnDisconnectedArgs,
  OnGroupDataMessageArgs,
  OnRejoinGroupFailedArgs,
  OnServerDataMessageArgs,
  OnStoppedArgs,
  WebPubSubClient
} from "@azure/web-pubsub-client"
import { createContext, JSX } from "preact"
import { useContext, useEffect, useMemo, useState } from "preact/hooks"
import { useApi } from "./useApi"
import { EventManager, IEventManager } from "../managers/EventManager"

const WebsocketContext = createContext<IEventManager>(null)

export function useWebsocket() {
  const manager = useContext(WebsocketContext)

  useEffect(() => () => manager.clear(), [manager])

  return manager
}

type WebsocketProviderProps = {
  children: JSX.Element
}

export function WebsocketProvider({ children }: WebsocketProviderProps) {
  const api = useApi()

  const [ready, setReady] = useState(false)

  const client = useMemo(
    () =>
      new WebPubSubClient({
        getClientAccessUrl: () => api.negotiate().then(res => res.url)
      }),
    [api]
  )

  useEffect(() => {
    client.start().then(() => setReady(true))
    return () => client.stop()
  }, [client])

  const manager = new EventManager()

  useEffect(() => {
    if (!ready) return

    const onConnected = (e: OnConnectedArgs) => manager.emit("connected", e)
    const onDisconnected = (e: OnDisconnectedArgs) => manager.emit("disconnected", e)
    const onStopped = (e: OnStoppedArgs) => manager.emit("stopped", e)
    const onRejoinGroupFailed = (e: OnRejoinGroupFailedArgs) => manager.emit("rejoin-group-failed", e)
    const onGroupMessage = (e: OnGroupDataMessageArgs) => manager.emit("group-message", e)
    const onServerMessage = (e: OnServerDataMessageArgs) => manager.emit("server-message", e)

    client.on("connected", onConnected)
    client.on("disconnected", onDisconnected)
    client.on("stopped", onStopped)
    client.on("rejoin-group-failed", onRejoinGroupFailed)
    client.on("group-message", onGroupMessage)
    client.on("server-message", onServerMessage)

    return () => {
      client.off("connected", onConnected)
      client.off("disconnected", onDisconnected)
      client.off("stopped", onStopped)
      client.off("rejoin-group-failed", onRejoinGroupFailed)
      client.off("group-message", onGroupMessage)
      client.off("server-message", onServerMessage)
    }
  }, [ready, client, manager])

  return <WebsocketContext.Provider value={manager}>{children}</WebsocketContext.Provider>
}