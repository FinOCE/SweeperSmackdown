namespace SweeperSmackdown.Bot.Commands

open SweeperSmackdown.Bot.Types
open SweeperSmackdown.Bot.Requests
open System.Threading.Tasks

type ICommand =
    abstract member Data: CreateGlobalApplicationCommand

    abstract member Execute: interaction: Interaction -> Task<Result<InteractionCallback, string>>
