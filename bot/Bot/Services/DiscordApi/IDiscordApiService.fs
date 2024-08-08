namespace SweeperSmackdown.Bot.Services

open SweeperSmackdown.Bot.Types
open SweeperSmackdown.Bot.Requests
open System.Threading.Tasks

type IDiscordApiService =
    abstract member CreateGlobalApplicationCommand:
        applicationId: string ->
        payload: CreateGlobalApplicationCommand ->
        Task<ApplicationCommand>

    abstract member BulkOverwriteGlobalApplicationCommands:
        applicationId: string -> 
        payload: CreateGlobalApplicationCommand list ->
        Task<ApplicationCommand list>

    abstract member CreateChannelInvite:
        channelId: string ->
        payload: CreateChannelInvite ->
        Task<Invite>
