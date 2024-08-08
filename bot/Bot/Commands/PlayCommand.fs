namespace SweeperSmackdown.Bot.Commands

open SweeperSmackdown.Bot.Types
open SweeperSmackdown.Bot.Requests
open SweeperSmackdown.Bot.Services

type PlayCommandValidationResult = { ChannelId: string }

type PlayCommand (configurationService: IConfigurationService, discordApiService: IDiscordApiService) =
    member _.Validate (interaction: Interaction) = 
        match interaction with
        | { Data = Some { Options = Some (Options.Channel "channel" channelId) } } -> Ok { ChannelId = channelId }
        | { Data = Some { Options = Some _ } } -> Error "Missing channel option in interaction data"
        | { Data = Some { Options = None } } -> Error "No options provided in interaction data"
        | { Data = None } -> Error "Missing interaction data"

    member _.Execute ({ ChannelId = channelId }: PlayCommandValidationResult) = task {
        let! invite =
            discordApiService.CreateChannelInvite
                channelId
                (CreateChannelInvite.build(
                    maxAge = 0,
                    targetType = InviteTargetType.EMBEDDED_APPLICATION,
                    targetApplicationId = configurationService.GetValue "DISCORD_APPLICATION_ID"
                ))

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

    interface ICommand with
        member _.Data = CreateGlobalApplicationCommand.build(
            Name = "play",
            Description = "Launch Sweeper Smackdown to play in any channel",
            Options = [
                ApplicationCommandOption.build(
                    Type = ApplicationCommandOptionType.CHANNEL,
                    Name = "channel",
                    Description = "The channel to launch Sweeper Smackdown in"
                )
            ]
        )

        member this.Execute interaction = task {
            match this.Validate interaction with
            | Error message -> return Error $"Validation error: {message}"
            | Ok validationResult -> return! this.Execute validationResult
        }
