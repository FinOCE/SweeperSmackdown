import {
  OnConnectedArgs,
  OnDisconnectedArgs,
  OnGroupDataMessageArgs,
  OnRejoinGroupFailedArgs,
  OnServerDataMessageArgs,
  OnStoppedArgs
} from "@azure/web-pubsub-client"

export namespace Http {
  export type Negotiate = {
    baseUrl: string
    url: string
    accessToken: string
  }
}

export namespace Ws {
  type EventHandler<T> = (e: T) => void

  type Event = {
    connected: EventHandler<OnConnectedArgs>
    disconnected: EventHandler<OnDisconnectedArgs>
    stopped: EventHandler<OnStoppedArgs>
    "server-message": EventHandler<OnServerDataMessageArgs>
    "group-message": EventHandler<OnGroupDataMessageArgs>
    "rejoin-group-failed": EventHandler<OnRejoinGroupFailedArgs>
  }

  export type Listener = { [K in keyof Event]: { event: K; handler: Event[K] } }[keyof Event]
}
