using Newtonsoft.Json;

namespace SweeperSmackdown.Stats.Models;

[TestClass]
public class PlayerInfoTests
{
    [TestMethod]
    public void Deserialize_AddsDefaultValuesForMissingProperties()
    {
        // Arrange
        var userId = "userId";
        var json = $"{{\"id\":\"{userId}\"}}";

        // Act
        var playerInfo = JsonConvert.DeserializeObject<PlayerInfo>(json)!;
        var serialized = JsonConvert.SerializeObject(playerInfo);
        var res = JsonConvert.DeserializeObject<dynamic>(serialized)!;

        Console.WriteLine("Original:   " + json);
        Console.WriteLine("Serialized: " + serialized);

        // Assert
        Assert.AreEqual(userId, playerInfo.Id);
        Assert.AreEqual(0, playerInfo.MinesFlagged);
        Assert.AreEqual(userId, (string)res["id"]);
        Assert.AreEqual(0, (int)res["minesFlagged"]);
    }
}
