namespace SweeperSmackdown.Bot.Services

open Microsoft.VisualStudio.TestTools.UnitTesting
open Microsoft.Extensions.Configuration
open NSubstitute
open NSubstitute.ReturnsExtensions
open System

[<TestClass>]
type ConfigurationServiceTests () =
    [<DefaultValue>] val mutable _key: string
    [<DefaultValue>] val mutable _value: string
    [<DefaultValue>] val mutable _configuration: IConfiguration

    [<TestInitialize>]
    member this.TestInitialize () =
        this._key <- "key"
        this._value <- "value"
        this._configuration <- Substitute.For<IConfiguration>()

    [<TestMethod>]
    member this.TryGetValue_InvalidKey_ReturnsNone () =
        // Arrange
        this._configuration[this._key].ReturnsNull() |> ignore
        let configurationService: IConfigurationService = ConfigurationService this._configuration

        // Act
        let res = configurationService.TryGetValue this._key

        // Assert
        Assert.AreEqual(None, res)

    [<TestMethod>]
    member this.TryGetValue_ValidKey_ReturnsCorrectValue () =
        // Arrange
        this._configuration[this._key].Returns(this._value) |> ignore
        let configurationService: IConfigurationService = ConfigurationService this._configuration

        // Act
        let res = configurationService.TryGetValue this._key

        // Assert
        Assert.AreEqual(this._value, res.Value)

    [<TestMethod>]
    member this.GetValue_InvalidKey_ThrowsException () =
        // Arrange
        this._configuration[this._key].Returns(fun _ -> failwith "") |> ignore
        let configurationService: IConfigurationService = ConfigurationService this._configuration

        // Act
        let res () = configurationService.GetValue this._key |> ignore

        // Assert
        Assert.ThrowsException(Action res) |> ignore

    [<TestMethod>]
    member this.GetValue_ValidKey_ReturnsCorrectValue () =
        // Arrange
        this._configuration[this._key].Returns(this._value) |> ignore
        let configurationService: IConfigurationService = ConfigurationService this._configuration

        // Act
        let res = configurationService.GetValue this._key

        // Assert
        Assert.AreEqual(this._value, res)
