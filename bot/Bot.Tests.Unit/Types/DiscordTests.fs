namespace SweeperSmackdown.Bot.Types

open FSharp.Json
open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type DiscordTests() =
    [<TestMethod>]
    member this.Deserialization_CorrectlyDeserializesTypes() =
        // Arrange
        let json = """{"type":4,"data":{"content":"test","allowed_mentions":{"parse":[]}}}"""

        let expected = InteractionCallback.Build(
            InteractionCallbackType.CHANNEL_MESSAGE_WITH_SOURCE,
            InteractionCallbackMessageData.Build(
                Content = "test",
                AllowedMentions = AllowedMentions.Build(
                    Parse = []
                )
            )
        )

        // Act
        let actual = Json.deserialize<InteractionCallback> json

        // Assert
        Assert.AreEqual(expected.Type, actual.Type)
        Assert.AreEqual(expected.Data.Value.Content, actual.Data.Value.Content)
        
    [<TestMethod>]
    member this.Serialization_CorrectlySerializesTypes() =
        // Arrange
        let callback = InteractionCallback.Build(
            InteractionCallbackType.CHANNEL_MESSAGE_WITH_SOURCE,
            InteractionCallbackMessageData.Build(
                Content = "test",
                AllowedMentions = AllowedMentions.Build(
                    Parse = []
                )
            )
        )

        let expected = """{"type":4,"data":{"content":"test","allowed_mentions":{"parse":[]}}}"""

        // Act
        let actual = Json.serializeEx (JsonConfig.create(true, SerializeNone.Omit)) callback

        // Assert
        Assert.AreEqual(expected, actual)
