using System;

namespace SweeperSmackdown.Strategies.Auth;

public static class AuthStrategyProvider
{
    public static IAuthStrategy GetStrategy(string type)
    {
        switch (type)
        {
            case "anonymous":
                return new AnonymousAuthStrategy();
            case "discord":
                return new DiscordAuthStrategy();
            default:
                throw new ArgumentException($"Invalid auth type provided ({type})");
        }
    }
}
