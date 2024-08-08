namespace SweeperSmackdown.Bot.Commands

open SweeperSmackdown.Bot.Types
open SweeperSmackdown.Bot.Requests
open SweeperSmackdown.Bot.Services

type PlayCommandValidationResult = { ChannelId: string }

type PlayCommand (configurationService: IConfigurationService, discordApiService: IDiscordApiService) =
    interface ICommand<PlayCommandValidationResult> with
        member _.Name = "play"

        member _.Validate interaction = 
            match interaction with
            | { Data = Some { Options = Some (Options.Channel "channel" channelId) } } -> Ok { ChannelId = channelId }
            | { Data = Some { Options = Some _ } } -> Error "Missing channel option in interaction data"
            | { Data = Some { Options = None } } -> Error "No options provided in interaction data"
            | { Data = None } -> Error "Missing interaction data"

        member _.Execute validationResult = task {
            let payload = CreateChannelInvite.build(
                maxAge = 0,
                targetType = InviteTargetType.EMBEDDED_APPLICATION,
                targetApplicationId = configurationService.GetValue "DISCORD_APPLICATION_ID"
            )

            let! invite = discordApiService.CreateChannelInvite validationResult.ChannelId payload

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
