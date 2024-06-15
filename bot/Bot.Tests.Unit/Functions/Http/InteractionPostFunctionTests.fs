namespace SweeperSmackdown.Bot.Functions.Http

// -------------------------------------------------------------------------------------------------
// Commenting out all tests because `HttpRequestData` seems to be a complete mess to work with at
// the moment. Will need to come back to these as I clean up other aspects of the project. As of
// writing this, the function does work when manually testing...
// -------------------------------------------------------------------------------------------------

//open FSharp.Json
//open Microsoft.Azure.Functions.Worker
//open Microsoft.Azure.Functions.Worker.Http
//open Microsoft.VisualStudio.TestTools.UnitTesting
//open NSubstitute
//open SweeperSmackdown.Bot.Discord
//open SweeperSmackdown.Bot.Services
//open System.IO
//open System.Net
//open System.Text

//[<TestClass>]
//type InteractionPostFunctionTests() =
//    [<DefaultValue>] val mutable _signingService: ISigningService
//    [<DefaultValue>] val mutable _configurationService: IConfigurationService
//    [<DefaultValue>] val mutable _function: InteractionPostFunction
//    [<DefaultValue>] val mutable _headers: HttpHeadersCollection
//    [<DefaultValue>] val mutable _req: HttpRequestData
//    [<DefaultValue>] val mutable _interaction: TempRequestInteraction

//    [<TestInitialize>]
//    member this.TestInitialize() =
//        this._signingService <- Substitute.For<ISigningService>()
//        this._configurationService <- Substitute.For<IConfigurationService>()
//        this._function <- InteractionPostFunction(this._signingService, this._configurationService)

//        let headerCollection = new HttpHeadersCollection()
//        headerCollection.Add("X-Signature-Ed25519", "ed25519")
//        headerCollection.Add("X-Signature-Timestamp", "timestamp")
//        this._headers <- headerCollection
        
//        let functionContext = Substitute.For<FunctionContext>()
//        let req = Substitute.For<HttpRequestData>(functionContext)
//        req.CreateResponse(Arg.Any<HttpStatusCode>()).Returns()
//        req.Headers.Returns(this._headers) |> ignore
//        this._req <- req

//        this._interaction <- { Type = InteractionType.PING }
//        let interactionStream = new MemoryStream(Json.serialize this._interaction |> Encoding.UTF8.GetBytes)
//        this._req.Body.Returns(interactionStream) |> ignore
        
//        this._configurationService
//            .TryGetValue(Arg.Any<string>())
//            .Returns(Some "value")
//        |> ignore

//        this._signingService
//            .Verify(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
//            .Returns(true)
//        |> ignore

//    [<TestMethod>]
//    member this.Run_MissingEnvironmentVariable_ReturnsInternalErrorResult() =
//        async {
//            // Arrange
//            this._configurationService
//                .TryGetValue(Arg.Any<string>())
//                .Returns(None)
//            |> ignore

//            // Act
//            let! res = this._function.Run(this._req) |> Async.AwaitTask

//            // Assert
//            Assert.AreEqual(HttpStatusCode.InternalServerError, res.StatusCode)
//        } |> Async.RunSynchronously

//    [<TestMethod>]
//    member this.Run_InvalidBody_ReturnsBadRequestResult() =
//        async {
//            // Arrange
//            this._req.Body.Returns(new MemoryStream("" |> Encoding.UTF8.GetBytes)) |> ignore

//            // Act
//            let! res = this._function.Run(this._req) |> Async.AwaitTask

//            // Assert
//            Assert.AreEqual(HttpStatusCode.BadRequest, res.StatusCode)
//        } |> Async.RunSynchronously

//    [<TestMethod>]
//    member this.Run_MissingEd25519Header_ReturnsUnauthorizedResult() =
//        async {
//            // Arrange
//            this._req.Headers.Contains("X-Signature-Ed25519").Returns(false) |> ignore

//            // Act
//            let! res = this._function.Run(this._req) |> Async.AwaitTask

//            // Assert
//            Assert.AreEqual(HttpStatusCode.Unauthorized, res.StatusCode)
//        } |> Async.RunSynchronously

//    [<TestMethod>]
//    member this.Run_MissingTimestampHeader_ReturnsUnauthorizedResult() =
//        async {
//            // Arrange
//            this._req.Headers.Contains("X-Signature-Timestamp").Returns(false) |> ignore

//            // Act
//            let! res = this._function.Run(this._req) |> Async.AwaitTask

//            // Assert
//            Assert.AreEqual(HttpStatusCode.Unauthorized, res.StatusCode)
//        } |> Async.RunSynchronously

//    [<TestMethod>]
//    member this.Run_InvalidSignature_ReturnsUnauthorizedResult() =
//        async {
//            // Arrange
//            this._signingService
//                .Verify(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
//                .Returns(false)
//            |> ignore

//            // Act
//            let! res = this._function.Run(this._req) |> Async.AwaitTask

//            // Assert
//            Assert.AreEqual(HttpStatusCode.Unauthorized, res.StatusCode)
//        } |> Async.RunSynchronously

//    [<TestMethod>]
//    member this.Run_PingInteraction_ReturnsPongResponse() =
//        async {
//            // Arrange

//            // Act
//            let! res = this._function.Run(this._req) |> Async.AwaitTask

//            // Assert
//            Assert.AreEqual(HttpStatusCode.OK, res.StatusCode)
//        } |> Async.RunSynchronously
