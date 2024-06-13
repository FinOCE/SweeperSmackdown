namespace SweeperSmackdown.Bot.Services

open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type ConfigurationServiceTests() =
    [<TestMethod>]
    [<Ignore>]
    member _.ReadOrThrow_InvalidKey_ThrowsException() =
        Assert.IsTrue(true) // TODO

    [<TestMethod>]
    [<Ignore>]
    member _.ReadOrThrow_ValidKey_ReturnsCorrectValue() =
        Assert.IsTrue(true) // TODO
