using System;

namespace SweeperSmackdown.Utils;

public static class VoteUtils
{
    /// <summary>
    /// Calculate the number of required votes based on the number of users participating.
    /// </summary>
    /// <param name="userCount">The number of users participating</param>
    /// <returns>The number of required votes</returns>
    public static int CalculateRequiredVotes(int userCount) =>
        Math.Max(1, (int)Math.Ceiling(userCount / 2d));
}
