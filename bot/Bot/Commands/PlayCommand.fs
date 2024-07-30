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
        | None -> Error(MissingApplicationId) |> Task.FromResult
        | Some applicationId ->

        // Ensure interaction contains data
        match interaction.Data with
        | None -> Error(MissingData) |> Task.FromResult
        | Some data ->

        // Ensure data contains options
        match data.Options with
        | None -> Error(MissingOption) |> Task.FromResult
        | Some options ->

        // Get channel option from options
        let option = options |> Seq.tryFind(fun o -> o.Type = ApplicationCommandOptionType.CHANNEL)

        if option.IsNone then
            Error(NoChannelOption) |> Task.FromResult
        else

        // Ensure option has a value
        match option.Value.Value with
        | None -> Error(NoOptionValue) |> Task.FromResult
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
                        Content = Some $"https://discord.gg/{invite.Code}";
                    };
                })

                // TODO: Figure out how to remove all the unnecessary `None` (ctors for types?)
            }
        | _ -> Error(NotStringValue) |> Task.FromResult

        // TODO: Figure out how to clean up all these non-indented match cases
