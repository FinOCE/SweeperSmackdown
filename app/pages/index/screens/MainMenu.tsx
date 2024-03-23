import { useWebsocket } from "../hooks/useWebsocket"
import "./MainMenu.scss"
import { OnGroupDataMessageArgs } from "@azure/web-pubsub-client"

export default function MainMenu() {
  const ws = useWebsocket({
    event: "group-message",
    handler: (e: OnGroupDataMessageArgs) => console.log("From user ID:", e.message.fromUserId)
  })

  return <div>Hello world</div>
}
