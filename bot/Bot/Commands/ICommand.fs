namespace SweeperSmackdown.Bot.Commands

open SweeperSmackdown.Bot.Types
open System.Threading.Tasks

type ICommand<'T> =
    abstract member Name: string

    abstract member Validate: interaction: Interaction -> Result<'T, string>

    abstract member Execute: validationResult: 'T -> Task<Result<InteractionCallback, string>>
