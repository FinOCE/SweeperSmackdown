namespace SweeperSmackdown.Bot.Commands

open FSharp.Json
open SweeperSmackdown.Bot.Types
open System.Net.Http

module PlayCommand = 
    type Failure =
        | MissingData
        | MissingOption
        | NotChannelOption

    type Invite = {
        [<JsonField("code")>]
        Code: string

        // TODO: Create full type and move into Discord API response types
    }

    let Run (interaction: Interaction) = task {
        match interaction.Data with
            | None -> return Error(MissingData)
            | Some data ->
                match data.Options with
                | None -> return Error(MissingOption)
                | Some options ->
                    let option = options |> Seq.tryFind(fun o -> o.Type = ApplicationCommandOptionType.CHANNEL)
                    match option with
                    | None -> return Error(NotChannelOption)
                    | Some option ->
                        let client = new HttpClient()
                        let req = new HttpRequestMessage(HttpMethod.Post, "") // TODO: Use correct path for creating invite

                        req.Headers.Clear()
                        req.Headers.Add("Authorization", "Bearer ???") // TODO: Add bot token here

                        let! res = client.SendAsync req
                        let! body = res.Content.ReadAsStringAsync()
                        let data = Json.deserialize<Invite> body

                        return Ok({
                            Type = InteractionCallbackType.CHANNEL_MESSAGE_WITH_SOURCE;
                            Data = Some {
                                Tts = None;
                                Embeds = None;
                                Flags = None;
                                Components = None;
                                Attachments = None;
                                Poll = None;
                                AllowedMentions = Some {
                                    Parse = [];
                                    Roles = None;
                                    Users = None;
                                    RepliedUser = None;
                                };
                                Content = Some $"https://discord.gg/{data.Code}";
                            };
                        })

                        // TODO: Figure out how to remove all the `None` from above type (maybe they shouldn't even be options?)
                        // TODO: Figure out how to make this not so indented
    }
