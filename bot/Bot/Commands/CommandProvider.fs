namespace SweeperSmackdown.Bot.Commands

open SweeperSmackdown.Bot.Types

type CommandProvider (playCommand: PlayCommand) =
    interface ICommandProvider with
        member _.Execute (interaction: Interaction) = task {
            let getCommandName (interaction: Interaction) =
                match interaction with
                | { Data = Some { Name = name } } -> Ok name
                | { Data = None } -> Error "Missing interaction data"

            let command: ICommand option =
                match getCommandName interaction with
                | Ok "play" -> Some playCommand
                | _ -> None

            match command with
            | None -> return Error "Unknown command"
            | Some cmd -> return! cmd.Execute interaction
        }
