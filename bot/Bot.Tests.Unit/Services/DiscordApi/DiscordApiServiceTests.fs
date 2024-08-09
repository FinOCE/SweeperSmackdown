namespace SweeperSmackdown.Bot.Services

open Microsoft.VisualStudio.TestTools.UnitTesting
open NSubstitute
open SweeperSmackdown.Bot.Types.Discord

[<TestClass>]
type DiscordApiServiceTests () =
    [<DefaultValue>] val mutable _configurationService: IConfigurationService

    [<TestInitialize>]
    member this.TestInitialize () =
        this._configurationService <- Substitute.For<IConfigurationService>()

    //member this.Send_SerializesResponse () =
    // TODO: Use HttpClient factory to create client, inject into service, then write tests for Send

    member this.Content_BuildsStringContent () = task {
        // Arrange
        let payload = InteractionCallback.build InteractionCallbackType.PONG
        let expected = """{"type":1}"""

        let discordApiService = DiscordApiService this._configurationService

        // Act
        let content = discordApiService.Content payload
        
        // Assert
        Assert.IsTrue(content.IsSome)

        let! actual = content.Value.ReadAsStringAsync()
        Assert.AreEqual(expected, actual)
    }
