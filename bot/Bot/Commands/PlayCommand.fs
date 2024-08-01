namespace SweeperSmackdown.Bot.Commands

open SweeperSmackdown.Bot.Types
open SweeperSmackdown.Bot.Requests
open SweeperSmackdown.Bot.Services

type PlayCommandValidResult = { channelId: string }

module Options =
    let (|Channel|_|) (name: string) (options: CommandInteractionDataOption list) =
        options |> List.tryPick (fun option ->
            match option with
            | ({
                Value = Some(CommandInteractionDataOptionValue.String channelId);
                Type = ApplicationCommandOptionType.CHANNEL;
            }) when option.Name = name -> Some(channelId)
            | _ -> None
        )
        
        // TODO: Create versions of above for all option types and move to shared file

type PlayCommand (configurationService: IConfigurationService) =
    let validate (interaction: Interaction) =
        match interaction with
        | { Data = Some { Options = Some (Options.Channel "channel" channelId) } } -> Ok { channelId = channelId }
        | { Data = Some { Options = Some _ } } -> Error "Missing channel option in interaction data"
        | { Data = Some { Options = None } } -> Error "No options provided in interaction data"
        | { Data = None } -> Error "Missing interaction data"

    member _.Run (interaction: Interaction) = task {
        match validate interaction with
        | Error message -> return Error message
        | Ok ({ channelId = channelId }) ->
            let! invite = (CreateChannelInvite
                .Build(
                    maxAge = 0,
                    targetType = InviteTargetType.EMBEDDED_APPLICATION,
                    targetApplicationId = configurationService.GetValue "DISCORD_APPLICATION_ID"
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
