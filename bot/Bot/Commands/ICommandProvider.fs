namespace SweeperSmackdown.Bot.Commands

open SweeperSmackdown.Bot.Types
open System.Threading.Tasks

type ICommandProvider =
    abstract member Commands: ICommand list

    abstract member Execute: interaction: Interaction -> Task<Result<InteractionCallback, string>>
