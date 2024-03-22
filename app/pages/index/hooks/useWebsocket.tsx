import { WebPubSubClient } from "@azure/web-pubsub-client"
import { createContext, JSX } from "preact"
import { useContext, useEffect, useState } from "preact/hooks"

const WebsocketContext = createContext<{
  client: WebPubSubClient
  ready: boolean
}>(null)

export const useWebsocket = () => useContext(WebsocketContext)

type WebsocketProviderProps = {
  children: JSX.Element
}

export function WebsocketProvider({ children }: WebsocketProviderProps) {
  const [ready, setReady] = useState(false)

  const client = new WebPubSubClient({
    getClientAccessUrl: async () =>
      fetch(`http://localhost:7071/negotiate`)
        .then(res => res.json())
        .then(data => data.url)
  })

  useEffect(() => {
    client.start().then(() => setReady(true))
    return () => client.stop()
  }, [client])

  return <WebsocketContext.Provider value={{ client, ready }}>{children}</WebsocketContext.Provider>
}
