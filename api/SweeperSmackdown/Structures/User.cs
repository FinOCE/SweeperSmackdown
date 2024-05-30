namespace SweeperSmackdown.Structures;

public class User
{
    public string Id { get; set; }

    public string DisplayName { get; set; }

    public string? AvatarUrl { get; set; }

    public User(string id, string displayName, string? avatarUrl)
    {
        Id = id;
        DisplayName = displayName;
        AvatarUrl = avatarUrl;
    }
}
