import { WebPubSubClient } from "@azure/web-pubsub-client"
import { createContext, JSX } from "preact"
import { useContext, useEffect, useState } from "preact/hooks"
import { Http, Ws } from "../types/Api"

const WebsocketContext = createContext<{
  client: WebPubSubClient
  ready: boolean
}>(null)

export function useWebsocket(...listeners: Ws.Listener[]) {
  const ctx = useContext(WebsocketContext)

  useEffect(() => {
    if (!ctx.ready) return

    // TODO: Figure out how to fix these typescript errors

    ///@ts-ignore
    listeners.forEach(listener => ctx.client.on(listener.event, listener.handler))

    ///@ts-ignore
    return () => listeners.forEach(listener => ctx.client.off(listener.event, listener.handler))
  }, [ctx.client, ctx.ready, listeners])

  return {
    joinGroup: ctx.client.joinGroup,
    leaveGroup: ctx.client.leaveGroup,
    sendEvent: ctx.client.sendEvent,
    sendToGroup: ctx.client.sendToGroup
  }
}

type WebsocketProviderProps = {
  children: JSX.Element
}

export function WebsocketProvider({ children }: WebsocketProviderProps) {
  const [ready, setReady] = useState(false)

  const client = new WebPubSubClient({
    getClientAccessUrl: async () =>
      fetch(`http://localhost:7071/negotiate`)
        .then(res => res.json())
        .then((data: Http.Negotiate) => data.url)
  })

  useEffect(() => {
    client.start().then(() => setReady(true))
    return () => client.stop()
  }, [client])

  return <WebsocketContext.Provider value={{ client, ready }}>{children}</WebsocketContext.Provider>
}
