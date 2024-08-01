namespace SweeperSmackdown.Bot.Services

open System.Net.Http
open System.Threading.Tasks

type Endpoint<'T> = {
    Method: HttpMethod
    Uri: string
}
with
    static member create(
        uri: string,
        ?Method: HttpMethod
    ) = {
        Uri = uri;
        Method =
            match Method with
            | Some method -> method
            | None -> HttpMethod.Get;
    }

type IDiscordApiService =
    abstract member Send<'T>: endpoint: Endpoint<'T> * ?body: HttpContent -> Task<'T>
