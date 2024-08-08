namespace SweeperSmackdown.Bot.Commands

open SweeperSmackdown.Bot.Types
open System.Threading.Tasks

type ICommand =
    abstract member Execute: interaction: Interaction -> Task<Result<InteractionCallback, string>>
