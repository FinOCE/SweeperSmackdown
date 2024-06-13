namespace SweeperSmackdown.Bot.Services

open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type Ed25519SigningServiceTests() =
    [<TestMethod>]
    [<DataRow("MESSAGE", "SIGNATURE", "PUBLIC_KEY")>]
    [<DataRow("MESSAGE", "SIGNATURE", "PUBLIC_KEY")>]
    [<Ignore>]
    member _.Verify_InvalidParameters_RejectsVerification(
        message: string,
        signature: string,
        publicKey: string
    ) =
        Assert.IsTrue(true) // TODO

    [<TestMethod>]
    [<DataRow("MESSAGE", "SIGNATURE", "PUBLIC_KEY")>]
    [<DataRow("MESSAGE", "SIGNATURE", "PUBLIC_KEY")>]
    [<Ignore>]
    member _.Verify_ValidParameters_AcceptsVerification(
        message: string,
        signature: string,
        publicKey: string
    ) =
        Assert.IsTrue(true) // TODO
