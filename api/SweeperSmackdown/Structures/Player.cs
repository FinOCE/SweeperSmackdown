namespace SweeperSmackdown.Structures;

public class Player
{
    public string Id { get; set; }

    public string LobbyId { get; set; }

    public string Name { get; set; }

    public string? AvatarUrl { get; set; }

    public bool Active { get; set; }

    public int Wins { get; set; }

    public int Score { get; set; }

    public Player(
        string id,
        string lobbyId,
        string? name = null,
        string? avatarUrl = null)
    {
        Id = id;
        LobbyId = lobbyId;
        Name = name ?? id;
        AvatarUrl = avatarUrl;
        Active = true;
        Wins = 0;
        Score = 0;
    }
}
