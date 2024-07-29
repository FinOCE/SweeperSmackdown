namespace SweeperSmackdown.Bot.Commands

open SweeperSmackdown.Bot.Types
open SweeperSmackdown.Bot.Requests

module PlayCommand = 
    type Failure =
        | MissingData
        | MissingOption
        | NotChannelOption

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
                        return! CreateChannelInvite
                            .Build(
                                maxAge = 0,
                                targetType = InviteTargetType.EMBEDDED_APPLICATION,
                                targetApplicationId = "" // TODO: Set actual application ID here
                            )
                            .SendAsync("") // TODO: Set channel ID here from `option` somehow

                        // TODO: Figure out how to make this not so indented                        
    }
