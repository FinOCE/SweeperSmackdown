namespace SweeperSmackdown.Bot.Services

open Org.BouncyCastle.Crypto.Parameters
open Org.BouncyCastle.Crypto.Signers
open System
open System.Text

type Ed25519SigningService() =
    interface ISigningService with
        member _.Verify(
            message: string,
            signature: string,
            publicKey: string
        ): bool =
            let messageBytes = message |> Encoding.UTF8.GetBytes
            let signatureBytes = signature |> Convert.FromHexString
            let publicKeyBytes = publicKey |> Encoding.UTF8.GetBytes

            let signer = Ed25519Signer()
            signer.Init(false, new Ed25519PublicKeyParameters(publicKeyBytes, 0))
            signer.BlockUpdate(messageBytes, 0, messageBytes.Length)
            
            signer.VerifySignature(signatureBytes)
