namespace SweeperSmackdown.Bot.Discord

open Microsoft.VisualStudio.TestTools.UnitTesting
open System.Text.Json

[<TestClass>]
type PingInteractionResponseDtoTests() =
    [<TestMethod>]
    member _.Type_IsCorrectValue() =
        // Arrange
        let dto = PingInteractionResponseDto()

        // Act

        // Assert
        Assert.AreEqual(InteractionCallbackType.PONG, dto.Type);

    [<TestMethod>]
    member _.SerializesCorrectly() =
        // Arrange
        let dto = PingInteractionResponseDto()
        let expected = JsonSerializer.Serialize({|
            ``type`` = InteractionCallbackType.PONG;
        |})

        // Act
        let json = JsonSerializer.Serialize(dto)

        // Assert
        Assert.AreEqual(expected, json)
