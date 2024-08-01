namespace SweeperSmackdown.Bot.Commands

open SweeperSmackdown.Bot.Types
open SweeperSmackdown.Bot.Requests
open SweeperSmackdown.Bot.Services

type PlayCommandValidResult = { channelId: string }

type PlayCommand (configurationService: IConfigurationService) =
    let validate (interaction: Interaction) =
        let (|Channel|_|) (name: string) (options: CommandInteractionDataOption list) =
            options
                |> Seq.map(fun o ->
                    match o with
                    | { Value = Some(CommandInteractionDataOptionValue.String channelId) }
                        when o.Type = ApplicationCommandOptionType.CHANNEL
                        && o.Name = name -> Some(channelId)
                    | _ -> None
                )
                |> Seq.filter(fun o -> o.IsSome)
                |> Seq.map(fun o -> o.Value)
                |> Seq.tryHead

        // TODO: Clean up `Seq` usage (I'm sure there's a cleaner way to do it)
        // TODO: Create versions of above for all option types and move to shared file

        match interaction with
        | { Data = Some { Options = Some (Channel "channel" channelId) } } -> Ok { channelId = channelId }
        | { Data = Some { Options = Some _ } } -> Error "No channel option in interaction data"
        | { Data = Some { Options = None } } -> Error "No options in interaction data"
        | { Data = None } -> Error "Missing interaction data"

    let run (interaction: Interaction) = task {
        match configurationService.TryGetValue "DISCORD_APPLICATION_ID" with
        | None -> return Error "Application missing discord application ID"
        | Some applicationId ->
            match validate interaction with
            | Error message -> return Error message
            | Ok ({ channelId = channelId }) ->
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