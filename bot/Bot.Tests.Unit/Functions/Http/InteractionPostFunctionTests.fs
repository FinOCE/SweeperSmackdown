namespace SweeperSmackdown.Bot.Functions.Http

open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type InteractionPostFunctionTests() =
    [<TestMethod>]
    [<Ignore>]
    member _.Run_InvalidBody_ReturnsBadRequestResult() =
        Assert.IsTrue(true) // TODO

    [<TestMethod>]
    [<Ignore>]
    member _.Run_InvalidSignature_ReturnsUnauthorizedResult() =
        Assert.IsTrue(true) // TODO

    [<TestMethod>]
    [<Ignore>]
    member _.Run_MissingEnvironmentVariable_ReturnsInternalErrorResult() =
        Assert.IsTrue(true) // TODO

    [<TestMethod>]
    [<Ignore>]
    member _.Run_PingInteraction_ReturnsPongResponse() =
        Assert.IsTrue(true) // TODO
