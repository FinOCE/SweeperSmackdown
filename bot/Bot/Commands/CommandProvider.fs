namespace SweeperSmackdown.Bot.Commands

open SweeperSmackdown.Bot.Types.Discord

type CommandProvider (
    playCommand: PlayCommand
) =
    let commands: ICommand list = [
        playCommand
    ]

    let getCommandName (interaction: Interaction) =
        match interaction with
        | { Data = None } -> None
        | { Data = Some { Name = name } } -> Some name

    let tryFindInteraction (commands: ICommand list) (name: string) =
        commands |> List.tryFind (fun c -> c.Data.Name = name)

    interface ICommandProvider with
        member _.Commands = commands

        member _.Execute interaction = task {
            match getCommandName interaction with
            | None -> return Error "Missing command name in interaction data"
            | Some name ->
                match tryFindInteraction commands name with
                | None -> return Error "Unknown command"
                | Some command -> return! command.Execute interaction
        }
