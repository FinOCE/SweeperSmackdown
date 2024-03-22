import { useEffect } from "preact/hooks"
import { useWebsocket } from "../hooks/useWebsocket"
import "./MainMenu.scss"
import { OnGroupDataMessageArgs } from "@azure/web-pubsub-client"

export default function MainMenu() {
  const { client, ready } = useWebsocket()

  useEffect(() => {
    const onGroupMessage = (e: OnGroupDataMessageArgs) => console.log("From user ID:", e.message.fromUserId)

    client.on("group-message", onGroupMessage)

    return () => {
      client.off("group-message", onGroupMessage)
    }
  }, [client])

  if (!ready) {
    return <div>Loading...</div>
  }

  return <div>Hello world</div>
}
