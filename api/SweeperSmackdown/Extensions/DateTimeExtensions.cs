using System;

namespace SweeperSmackdown.Extensions;

public static class DateTimeExtensions
{
    /// <summary>
    /// Convert a DateTime into the number of seconds since unix epoch (1/1/1970, 00:00).
    /// </summary>
    /// <param name="dateTime">The DateTime</param>
    /// <returns>The number of seconds since unix epoch</returns>
    public static long ToUnixTimeSeconds(this DateTime dateTime) =>
        ((DateTimeOffset)dateTime).ToUnixTimeSeconds();

    /// <summary>
    /// Convert a DateTime into the number of milliseconds since unix epoch (1/1/1970, 00:00).
    /// </summary>
    /// <param name="dateTime">The DateTime</param>
    /// <returns>The number of milliseconds since unix epoch</returns>
    public static long ToUnixTimeMilliseconds(this DateTime dateTime) =>
        ((DateTimeOffset)dateTime).ToUnixTimeMilliseconds();
}
