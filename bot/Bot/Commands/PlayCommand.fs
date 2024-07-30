namespace SweeperSmackdown.Bot.Commands

open SweeperSmackdown.Bot.Types
open SweeperSmackdown.Bot.Requests
open SweeperSmackdown.Bot.Services
open System.Threading.Tasks

type PlayCommandFailure =
    | MissingApplicationId
    | MissingData
    | MissingOption
    | NoChannelOption
    | NoOptionValue
    | NotStringValue

type PlayCommand (configurationService: IConfigurationService) =
    let Run (interaction: Interaction) =
        // Get the application ID from the environment
        match configurationService.TryGetValue "DISCORD_APPLICATION_ID" with
        | None -> Task.FromResult <| Error MissingApplicationId
        | Some applicationId ->

        // Ensure interaction contains data
        match interaction.Data with
        | None -> Task.FromResult <| Error MissingData
        | Some data ->

        // Ensure data contains options
        match data.Options with
        | None -> Task.FromResult <| Error MissingOption
        | Some options ->

        // Get channel option from options
        let option = options |> Seq.tryFind(fun o -> o.Type = ApplicationCommandOptionType.CHANNEL)

        if option.IsNone then
            Task.FromResult <| Error NoChannelOption
        else

        // Ensure option has a value
        match option.Value.Value with
        | None -> Task.FromResult <| Error NoOptionValue
        | Some value -> 
                            
        // Ensure value is a string
        match value with
        | CommandInteractionDataOptionValue.String channelId ->
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
        | _ -> Task.FromResult <| Error NotStringValue

        // TODO: Figure out how to clean up all these non-indented match cases
