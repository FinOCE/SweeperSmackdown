namespace SweeperSmackdown.Utils;

public static class DiscordUtils
{
    public static string GetAvatarUrl(string id, string discriminator, string? avatar)
    {
        if (avatar is null)
        {
            var defaultAvatarIndex = discriminator == "0"
                ? (int.Parse(id) >> 22)
                : (int.Parse(discriminator) % 5);

            return $"https://cdn.discordapp.com/embed/avatars/{defaultAvatarIndex}.png";
        }
        else
        {
            string imageType = avatar.StartsWith("a_")
                ? "gif"
                : "png";

            return $"https://cdn.discordapp.com/avatars/{id}/{avatar}.{imageType}";
        }
    }
}
