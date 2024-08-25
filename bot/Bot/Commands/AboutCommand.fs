namespace SweeperSmackdown.Bot.Commands

open SweeperSmackdown.Bot.Types.Discord
open System.Threading.Tasks

type AboutCommand () =
    member _.Validate (interaction: Interaction) = 
        match interaction with
        | { Data = None } -> Error "Missing interaction data"
        | _ -> Ok 0

    member _.Execute () =
        let embed = Embed.build(
            Title = "Sweeper Smackdown",
            Description = "TODO: Write a meaningful description and style this embed"
        )

        // Could consider just sending a neat link to the website with a bigger blurb and whatnot.
        // Probably not as good, but maybe that in conjunction with a good embed. Base embed design
        // off embed in #design in personal server for inspiration

        InteractionCallback.build(
            Type = InteractionCallbackType.CHANNEL_MESSAGE_WITH_SOURCE,
            Data = InteractionCallbackMessageData.buildBase(
                Embeds = [embed]
            )
        )

    interface ICommand with
        member _.Data = CreateGlobalApplicationCommand.build(
            Name = "about",
            Description = "Learn about Sweeper Smackdown and how to play"
        )

        member this.Execute interaction =
            match this.Validate interaction with
            | Error message -> Error $"Validation error: {message}"
            | Ok _ -> Ok <| this.Execute()
            |> Task.FromResult
