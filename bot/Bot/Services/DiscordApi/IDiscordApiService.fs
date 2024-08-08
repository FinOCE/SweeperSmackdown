namespace SweeperSmackdown.Bot.Services

open SweeperSmackdown.Bot.Types
open SweeperSmackdown.Bot.Requests
open System.Threading.Tasks

type IDiscordApiService =
    abstract member CreateChannelInvite: channelId: string -> payload: CreateChannelInvite -> Task<Invite>
