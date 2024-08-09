namespace SweeperSmackdown.Bot.Types.Discord

open FSharp.Json
open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type DiscordTests() =
    [<TestMethod>]
    member this.Deserialization_CorrectlyDeserializesTypes() =
        // Arrange
        let json = """{"type":4,"data":{"content":"test","allowed_mentions":{"parse":[]}}}"""

        let expected = InteractionCallback.build(
            InteractionCallbackType.CHANNEL_MESSAGE_WITH_SOURCE,
            InteractionCallbackMessageData.buildBase(
                Content = "test",
                AllowedMentions = AllowedMentions.build(
                    Parse = []
                )
            )
        )

        // Act
        let actual = Json.deserialize<InteractionCallback> json

        // TODO: Figure out how to fix deserialization of InteractionCallbackData union
        //
        // Seems like it may be to do with it expecting nulls? Says its failing because it was
        // expecting one field not two, so probably looking at choice data instead. I would assume
        // using a `Json.deserialize` with a configuration to handle nulls properly will work. This
        // would affect serializing the body of an interaction POST, so essential to be figured out.

        // Assert
        Assert.AreEqual(expected.Type, actual.Type)

        match (expected.Data, actual.Data) with
        | Some (InteractionCallbackData.InteractionCallbackMessageData expected),
          Some (InteractionCallbackData.InteractionCallbackMessageData actual) -> 
            Assert.AreEqual(expected.Content, actual.Content)
        | _ ->
            Assert.Fail()
        
    [<TestMethod>]
    member this.Serialization_CorrectlySerializesTypes() =
        // Arrange
        let callback = InteractionCallback.build(
            InteractionCallbackType.CHANNEL_MESSAGE_WITH_SOURCE,
            InteractionCallbackMessageData.buildBase(
                Content = "test",
                AllowedMentions = AllowedMentions.build(
                    Parse = []
                )
            )
        )

        let expected = """{"type":4,"data":{"content":"test","allowed_mentions":{"parse":[]}}}"""

        // Act
        let actual = Json.serializeEx (JsonConfig.create(true, SerializeNone.Omit)) callback

        // Assert
        Assert.AreEqual(expected, actual)
