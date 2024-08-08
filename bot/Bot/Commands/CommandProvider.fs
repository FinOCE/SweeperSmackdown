namespace SweeperSmackdown.Bot.Commands

open SweeperSmackdown.Bot.Types

type CommandProvider (playCommand: PlayCommand) =
    let commands: ICommand<_> list = [playCommand]

    let getCommandName (interaction: Interaction) =
        match interaction with
        | { Data = None } -> None
        | { Data = Some { Name = name } } -> Some name

    let tryFindInteraction (name: string) =
        commands |> List.tryFind (fun c -> c.Name = name)

    interface ICommandProvider with
        member _.Execute (interaction: Interaction) = task {
            match getCommandName interaction with
            | None -> return Error "Missing command name in interaction data"
            | Some name ->
                match tryFindInteraction name with
                | None -> return Error "Unknown command"
                | Some command -> 
                    match command.Validate interaction with
                    | Error message -> return Error $"Validation error: {message}"
                    | Ok validationResult -> return! command.Execute validationResult
        }
