namespace SweeperSmackdown.Bot.Services

open System
open System.Text
open TweetNaclSharp

type Ed25519SigningService() =
    interface ISigningService with
        member _.Verify(
            timestamp: string,
            body: string,
            signature: string,
            publicKey: string
        ): bool =
            Nacl.SignDetachedVerify(
                timestamp + body |> Encoding.UTF8.GetBytes,
                signature |> Convert.FromHexString,
                publicKey |> Convert.FromHexString
            )
