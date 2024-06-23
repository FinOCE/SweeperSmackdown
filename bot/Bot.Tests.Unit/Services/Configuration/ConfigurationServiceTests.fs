namespace SweeperSmackdown.Bot.Services

open Microsoft.VisualStudio.TestTools.UnitTesting
open Microsoft.Extensions.Configuration
open NSubstitute
open NSubstitute.ReturnsExtensions

[<TestClass>]
type ConfigurationServiceTests() =
    [<DefaultValue>] val mutable _key: string
    [<DefaultValue>] val mutable _value: string
    [<DefaultValue>] val mutable _configuration: IConfiguration

    [<TestInitialize>]
    member this.TestInitialize() =
        this._key <- "key"
        this._value <- "value"
        this._configuration <- Substitute.For<IConfiguration>()

    [<TestMethod>]
    member this.ReadOrThrow_InvalidKey_ThrowsException() =
        // Arrange
        this._configuration[this._key].ReturnsNull() |> ignore
        let configurationService: IConfigurationService = ConfigurationService this._configuration

        // Act
        let res = configurationService.TryGetValue this._key

        // Assert
        Assert.AreEqual(None, res)

    [<TestMethod>]
    member this.ReadOrThrow_ValidKey_ReturnsCorrectValue() =
        // Arrange
        this._configuration[this._key].Returns(this._value) |> ignore
        let configurationService: IConfigurationService = ConfigurationService this._configuration

        // Act
        let res = configurationService.TryGetValue this._key

        // Assert
        Assert.AreEqual(this._value, res.Value)
