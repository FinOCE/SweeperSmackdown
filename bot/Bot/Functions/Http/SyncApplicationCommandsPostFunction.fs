namespace SweeperSmackdown.Bot.Functions.Http

open FSharp.Json
open Microsoft.Azure.Functions.Worker
open Microsoft.Azure.Functions.Worker.Http
open SweeperSmackdown.Bot.Commands
open SweeperSmackdown.Bot.Services
open System.Net

type SyncApplicationCommandsPostFunction (
    configurationService: IConfigurationService,
    discordApiService: IDiscordApiService,
    commandProvider: ICommandProvider
) =
    [<Function(nameof SyncApplicationCommandsPostFunction)>]
    member _.Run (
        [<HttpTrigger(AuthorizationLevel.Admin, "post", Route = "sync")>] req: HttpRequestData
    ) = task {
        let! commands =
            discordApiService.BulkOverwriteGlobalApplicationCommands
                (configurationService.GetValue "DISCORD_APPLICATION_ID")
                (commandProvider.Commands |> List.map (fun c -> c.Data))

        let res = req.CreateResponse HttpStatusCode.OK
        res.Headers.Add("Content-Type", "application/json")
        do! res.WriteStringAsync(Json.serialize commands)
        return res
    }
