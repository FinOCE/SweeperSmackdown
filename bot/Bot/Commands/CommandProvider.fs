namespace SweeperSmackdown.Bot.Commands

open SweeperSmackdown.Bot.Types

type CommandProvider (
    play: PlayCommand
) =
    let validate (interaction: Interaction) =
        match interaction with
        | { Data = Some { Name = name } } -> Ok name
        | { Data = None } -> Error "Missing interaction data"

    interface ICommandProvider with
        member _.Execute (interaction: Interaction) = task {
            match validate interaction with
            | Ok "play" -> return! play.Run interaction
            | _ -> return Error "Unkown command"
        }
