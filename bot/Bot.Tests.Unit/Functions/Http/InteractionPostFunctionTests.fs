namespace SweeperSmackdown.Bot.Functions.Http

open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Mvc
open Microsoft.Azure.Functions.Worker
open Microsoft.Extensions.Logging
open Microsoft.VisualStudio.TestTools.UnitTesting
open NSubstitute
open SweeperSmackdown.Bot.Discord
open SweeperSmackdown.Bot.Services
open System.IO
open System.Text
open System.Text.Json
open Microsoft.Extensions.Primitives

[<TestClass>]
type InteractionPostFunctionTests() =
    [<DefaultValue>] val mutable _logger: ILogger<InteractionPostFunction>
    [<DefaultValue>] val mutable _signingService: ISigningService
    [<DefaultValue>] val mutable _configurationService: IConfigurationService
    [<DefaultValue>] val mutable _function: InteractionPostFunction
    [<DefaultValue>] val mutable _req: HttpRequest
    [<DefaultValue>] val mutable _executionContext: FunctionContext
    [<DefaultValue>] val mutable _interaction: Interaction

    [<TestInitialize>]
    member this.TestInitialize() =
        this._logger <- Substitute.For<ILogger<InteractionPostFunction>>()
        this._signingService <- Substitute.For<ISigningService>()
        this._configurationService <- Substitute.For<IConfigurationService>()
        this._function <- InteractionPostFunction(this._logger, this._signingService, this._configurationService)
        this._req <- Substitute.For<HttpRequest>()
        this._executionContext <- Substitute.For<FunctionContext>()
        
        this._req.Headers["X-Signature-Ed25519"].Returns("ed25519") |> ignore
        this._req.Headers["X-Signature-Timestamp"].Returns("timestamp") |> ignore

        this._interaction <- Interaction(Type = InteractionType.PING)
        let interactionStream = new MemoryStream(JsonSerializer.Serialize(this._interaction) |> Encoding.UTF8.GetBytes)
        this._req.Body.Returns(interactionStream) |> ignore

    [<TestMethod>]
    member this.Run_MissingEnvironmentVariable_ReturnsInternalErrorResult() =
        async {
            // Arrange
            this._configurationService
                .TryGetValue(Arg.Any<string>())
                .Returns(None)
            |> ignore

            // Act
            let! res = this._function.Run(this._req, this._executionContext, this._interaction) |> Async.AwaitTask

            // Assert
            Assert.IsInstanceOfType<StatusCodeResult>(res)
            Assert.AreEqual(500, (res :?> StatusCodeResult).StatusCode)
        } |> Async.RunSynchronously

    [<TestMethod>]
    member this.Run_MissingEd25519Header_ReturnsUnauthorizedResult() =
        async {
            // Arrange
            this._configurationService
                .TryGetValue(Arg.Any<string>())
                .Returns(Some "value")
            |> ignore

            this._req.Headers["X-Signature-Ed25519"].Returns(StringValues()) |> ignore

            // Act
            let! res = this._function.Run(this._req, this._executionContext, this._interaction) |> Async.AwaitTask

            // Assert
            Assert.IsInstanceOfType<StatusCodeResult>(res)
            Assert.AreEqual(401, (res :?> StatusCodeResult).StatusCode)
        } |> Async.RunSynchronously

    [<TestMethod>]
    member this.Run_MissingTimestampHeader_ReturnsUnauthorizedResult() =
        async {
            // Arrange
            this._configurationService
                .TryGetValue(Arg.Any<string>())
                .Returns(Some "value")
            |> ignore

            this._req.Headers["X-Signature-Timestamp"].Returns(StringValues()) |> ignore

            // Act
            let! res = this._function.Run(this._req, this._executionContext, this._interaction) |> Async.AwaitTask

            // Assert
            Assert.IsInstanceOfType<StatusCodeResult>(res)
            Assert.AreEqual(401, (res :?> StatusCodeResult).StatusCode)
        } |> Async.RunSynchronously

    [<TestMethod>]
    member this.Run_InvalidSignature_ReturnsUnauthorizedResult() =
        async {
            // Arrange
            this._configurationService
                .TryGetValue(Arg.Any<string>())
                .Returns(Some "value")
            |> ignore

            this._signingService
                .Verify(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(false)
            |> ignore

            // Act
            let! res = this._function.Run(this._req, this._executionContext, this._interaction) |> Async.AwaitTask

            // Assert
            Assert.IsInstanceOfType<StatusCodeResult>(res)
            Assert.AreEqual(401, (res :?> StatusCodeResult).StatusCode)
        } |> Async.RunSynchronously

    [<TestMethod>]
    member this.Run_PingInteraction_ReturnsPongResponse() =
        async {
            // Arrange
            this._configurationService
                .TryGetValue(Arg.Any<string>())
                .Returns(Some "value")
            |> ignore

            this._signingService
                .Verify(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(true)
            |> ignore

            // Act
            let! res = this._function.Run(this._req, this._executionContext, this._interaction) |> Async.AwaitTask

            // Assert
            Assert.IsInstanceOfType<OkObjectResult>(res)
        } |> Async.RunSynchronously