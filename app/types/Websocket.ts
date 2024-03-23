export namespace Websocket {
  export namespace Response {
    export type UserJoin = {
      eventName: "USER_JOIN"
      userId: string
      message: string
      content: string
    }
  }
}
