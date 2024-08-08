namespace SweeperSmackdown.Bot.Commands

open SweeperSmackdown.Bot.Types
open SweeperSmackdown.Bot.Requests
open SweeperSmackdown.Bot.Services

type PlayCommandValidationResult = { channelId: string }

type PlayCommand (configurationService: IConfigurationService, discordApiService: IDiscordApiService) =
    let validate (interaction: Interaction) =
        match interaction with
        | { Data = Some { Options = Some (Options.Channel "channel" channelId) } } -> Ok { channelId = channelId }
        | { Data = Some { Options = Some _ } } -> Error "Missing channel option in interaction data"
        | { Data = Some { Options = None } } -> Error "No options provided in interaction data"
        | { Data = None } -> Error "Missing interaction data"

    interface ICommand with
        member _.Execute (interaction: Interaction) = task {
            match validate interaction with
            | Error message -> return Error message
            | Ok ({ channelId = channelId }) ->
                let payload = CreateChannelInvite.build(
                    maxAge = 0,
                    targetType = InviteTargetType.EMBEDDED_APPLICATION,
                    targetApplicationId = configurationService.GetValue "DISCORD_APPLICATION_ID"
                )

                let! invite = discordApiService.CreateChannelInvite channelId payload

                return Ok (InteractionCallback.build(
                    Type = InteractionCallbackType.CHANNEL_MESSAGE_WITH_SOURCE,
                    Data = InteractionCallbackMessageData.build(
                        Content = $"https://discord.gg/{invite.Code}",
                        AllowedMentions = AllowedMentions.build(
                            Parse = []
                        )
                    )
                ))
        }
