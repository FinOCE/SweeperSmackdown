namespace SweeperSmackdown.Bot.Services

open Microsoft.VisualStudio.TestTools.UnitTesting
open Microsoft.Extensions.Configuration
open NSubstitute
open NSubstitute.ReturnsExtensions
open System.Configuration

[<TestClass>]
type ConfigurationServiceTests() =
    member val _configuration = Substitute.For<IConfiguration>()

    [<TestMethod>]
    member this.ReadOrThrow_InvalidKey_ThrowsException() =
        // Arrange
        let key = "key"

        this._configuration = Substitute.For<IConfiguration>() |> ignore

        this._configuration[key].ReturnsNull() |> ignore
        let configurationService: IConfigurationService = ConfigurationService this._configuration

        // Act
        let res = fun () -> configurationService.ReadOrThrow(key) |> ignore

        // Assert
        Assert.ThrowsException<SettingsPropertyNotFoundException>(res)

    [<TestMethod>]
    member this.ReadOrThrow_ValidKey_ReturnsCorrectValue() =
        // Arrange
        let key = "key"
        let value = "value"

        this._configuration[key].Returns(value) |> ignore
        let configurationService: IConfigurationService = ConfigurationService this._configuration

        // Act
        let res = configurationService.ReadOrThrow(key)

        // Assert
        Assert.AreEqual(value, res)
