namespace SweeperSmackdown.Bot.Services

open Microsoft.VisualStudio.TestTools.UnitTesting
open Org.BouncyCastle.Security
open Org.BouncyCastle.Crypto.Generators
open Org.BouncyCastle.Crypto.Parameters
open Org.BouncyCastle.OpenSsl
open System
open System.Text
open System.IO

[<TestClass>]
type Ed25519SigningServiceTests() =
    member val _iterations = 3

    [<TestMethod>]
    member this.TESTUTIL_GenerateValidParameters() =
        for _ = 1 to this._iterations do
            let message = Guid.NewGuid().ToByteArray() |> Convert.ToBase64String
            let data = message |> Encoding.UTF8.GetBytes

            let parameters = SecureRandom() |> Ed25519KeyGenerationParameters
            let generator = Ed25519KeyPairGenerator()
            generator.Init(parameters)
            let keypair = generator.GenerateKeyPair()

            let signer = SignerUtilities.GetSigner("Ed25519")
            signer.Init(true, keypair.Private)
            signer.BlockUpdate(data, 0, message.Length)
            let signature = signer.GenerateSignature() |> Convert.ToHexString

            let textWriter = new StringWriter()
            let pemWriter = PemWriter(textWriter)
            pemWriter.WriteObject(keypair.Public)
            pemWriter.Writer.Flush()
            let publicKey = textWriter.ToString()

            Console.WriteLine("Message: " + message)
            Console.WriteLine("Signature: " + signature)
            Console.WriteLine("Public key: " + publicKey)
            Console.WriteLine()

            // TODO: Determine how these should be generated to be valid (based off https://asecuritysite.com/bouncy/bc_eddsa)

    [<TestMethod>]
    [<DataRow("MESSAGE", "SIGNATURE", "PUBLIC_KEY")>] // TODO: Populate a couple data rows
    [<Ignore>]
    member _.Verify_InvalidParameters_RejectsVerification(
        message: string,
        signature: string,
        publicKey: string
    ) =
        // Arrange
        let signingService: ISigningService = Ed25519SigningService()

        // Act
        let res = signingService.Verify(message, signature, publicKey)

        // Assert
        Assert.IsFalse(res)

    [<TestMethod>]
    [<DataRow("MESSAGE", "SIGNATURE", "PUBLIC_KEY")>] // TODO: Populate a couple data rows
    [<Ignore>]
    member _.Verify_ValidParameters_AcceptsVerification(
        message: string,
        signature: string,
        publicKey: string
    ) =
        // Arrange
        let signingService: ISigningService = Ed25519SigningService()

        // Act
        let res = signingService.Verify(message, signature, publicKey)

        // Assert
        Assert.IsTrue(res)
