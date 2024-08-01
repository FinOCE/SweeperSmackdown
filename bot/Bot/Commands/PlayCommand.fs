namespace SweeperSmackdown.Bot.Commands

open SweeperSmackdown.Bot.Types
open SweeperSmackdown.Bot.Requests
open SweeperSmackdown.Bot.Services
open System.Threading.Tasks

type PlayCommand (configurationService: IConfigurationService) =
    let TaskError message = Task.FromResult <| Error message

    let (|Valid|NoChannelOption|NoOptions|MissingData|) (interaction: Interaction) =
        let (|Channel|_|) (options: CommandInteractionDataOption list) =
            match options |> Seq.tryFind(fun o -> o.Type = ApplicationCommandOptionType.CHANNEL) with
            | Some { Value = Some(CommandInteractionDataOptionValue.String channelId) } -> Some (Channel channelId)
            | _ -> None

        match interaction with
        | { Data = Some { Options = Some (Channel channelId) } } -> Valid channelId
        | { Data = Some { Options = Some _ } } -> NoChannelOption
        | { Data = Some { Options = None } } -> NoOptions
        | { Data = None } -> MissingData

    let Run (interaction: Interaction) =
        match configurationService.TryGetValue "DISCORD_APPLICATION_ID" with
        | None -> TaskError "Application missing discord application ID"
        | Some applicationId ->
            match interaction with
            | MissingData -> TaskError "Missing interaction data"
            | NoOptions -> TaskError "No options in interaction data"
            | NoChannelOption -> TaskError "No channel option in interaction data"
            | Valid channelId ->
                task {
                    let! invite = (CreateChannelInvite
                        .Build(
                            maxAge = 0,
                            targetType = InviteTargetType.EMBEDDED_APPLICATION,
                            targetApplicationId = applicationId
                        )
                        .SendAsync(channelId))

                    return Ok (InteractionCallback.Build(
                        InteractionCallbackType.CHANNEL_MESSAGE_WITH_SOURCE,
                        InteractionCallbackMessageData.Build(
                            Content = $"https://discord.gg/{invite.Code}",
                            AllowedMentions = AllowedMentions.Build(
                                Parse = []
                            )
                        )
                    ))
                }
                