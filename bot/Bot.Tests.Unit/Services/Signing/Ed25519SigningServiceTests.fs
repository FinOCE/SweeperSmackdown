namespace SweeperSmackdown.Bot.Services

open Microsoft.VisualStudio.TestTools.UnitTesting
open System
open System.Text
open TweetNaclSharp

[<TestClass>]
type Ed25519SigningServiceTests() =
    [<TestMethod>]
    member _.``TESTUTIL - Generate valid ed25519 datarows`` () =
        let body = Guid.NewGuid().ToString()
        let timestamp = DateTime.UtcNow.ToString()

        let keypair = Nacl.SignKeyPair()
        let publicKey = keypair.PublicKey |> Convert.ToHexString

        let message = timestamp + body |> Encoding.UTF8.GetBytes
        let signature = Nacl.SignDetached(message, keypair.SecretKey) |> Convert.ToHexString

        Console.WriteLine("Body:       " + body)
        Console.WriteLine("Timestamp:  " + timestamp)
        Console.WriteLine("Signature:  " + signature)
        Console.WriteLine("Public key: " + publicKey)

    [<TestMethod>]
    [<DataRow(
        "14/06/2024 3:33:11 PM",
        "e4e3e46e-99b1-4cc5-8fcb-a43ea68afaa6",
        "8228317DD172FCB211E6532D2CC34B7158D0877D768EAEA6A1205CEBEFF32CAE114455830E2885E1CAB7D54BF2ED387FD4F4D73E8AFAF186D2141F8778582901",
        "1DA73B9FFB48F89FB07D0226FB147A95C7DD412D985548822D3127416D505C06"
    )>]
    member _.Verify_InvalidParameters_RejectsVerification(
        timestamp: string,
        body: string,
        signature: string,
        publicKey: string
    ) =
        // Arrange
        let signingService: ISigningService = Ed25519SigningService()

        // Act
        let res = signingService.Verify(timestamp, body, signature, publicKey)

        // Assert
        Assert.IsFalse(res)

    [<TestMethod>]
    [<DataRow(
        "14/06/2024 3:33:11 PM",
        "e4e3e46e-99b1-4cc5-8fcb-a43ea68afaa6",
        "9228317DD172FCB211E6532D2CC34B7158D0877D768EAEA6A1205CEBEFF32CAE114455830E2885E1CAB7D54BF2ED387FD4F4D73E8AFAF186D2141F8778582901",
        "1DA73B9FFB48F89FB07D0226FB147A95C7DD412D985548822D3127416D505C06"
    )>]
    member _.Verify_ValidParameters_AcceptsVerification(
        timestamp: string,
        body: string,
        signature: string,
        publicKey: string
    ) =
        // Arrange
        let signingService: ISigningService = Ed25519SigningService()

        // Act
        let res = signingService.Verify(timestamp, body, signature, publicKey)

        // Assert
        Assert.IsTrue(res)
