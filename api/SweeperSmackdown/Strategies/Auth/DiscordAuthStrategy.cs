﻿using SweeperSmackdown.DTOs.Discord;
using System.Collections.Generic;
using System.Net.Http;
using System;
using System.Threading.Tasks;
using System.Linq;
using SweeperSmackdown.Utils;
using SweeperSmackdown.Structures;
using static Google.Protobuf.Collections.MapField<TKey, TValue>;

namespace SweeperSmackdown.Strategies.Auth;

public class DiscordAuthStrategy : IAuthStrategy
{
    public async Task<string> GenerateAccessToken(string code)
    {
        using HttpClient client = new();

        var body = new Dictionary<string, string>
            {
                { "client_id", Environment.GetEnvironmentVariable("DiscordClientId")! },
                { "client_secret", Environment.GetEnvironmentVariable("DiscordClientSecret")! },
                { "grant_type", "authorization_code" },
                { "code", code }
            };
        var encoded = string.Join("&", body.Select(kvp => $"{kvp.Key}={kvp.Value}"));

        var content = new StringContent(encoded);
        content.Headers.Clear();
        content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

        var res = await client.PostAsync("https://discord.com/api/oauth2/token", content);
        var data = await res.Content.ReadAsAsync<DiscordOAuthTokenResponseDto>();

        return data.AccessToken;
    }

    public async Task<string> Refresh(string refreshToken)
    {
        using HttpClient client = new();

        var body = new Dictionary<string, string>
            {
                { "client_id", Environment.GetEnvironmentVariable("DiscordClientId")! },
                { "client_secret", Environment.GetEnvironmentVariable("DiscordClientSecret")! },
                { "grant_type", "refresh_token" },
                { "refresh_token", refreshToken }
            };
        var encoded = string.Join("&", body.Select(kvp => $"{kvp.Key}={kvp.Value}"));

        var content = new StringContent(encoded);
        content.Headers.Clear();
        content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

        var res = await client.PostAsync("https://discord.com/api/oauth2/token", content);
        var data = await res.Content.ReadAsAsync<DiscordOAuthTokenResponseDto>();

        return data.AccessToken;
    }

    public async Task<User> GetUserInfo(string accessToken, string? guildId)
    {
        using HttpClient client = new();

        if (guildId is not null)
        {
            using HttpRequestMessage req = new(
                HttpMethod.Get,
                $"https://discord.com/api/users/@me/guilds/{guildId}/member");

            req.Headers.Clear();
            req.Headers.Add("Authorization", $"Bearer {accessToken}");

            var res = await client.SendAsync(req);
            var data = await res.Content.ReadAsAsync<DiscordOAuthGuildMemberResponseDto>();

            return new User(
                data.User.Id,
                data.Nickname ?? data.User.GlobalName ?? data.User.Username,
                DiscordUtils.GetAvatarUrl(data.User.Id, data.User.Discriminator, data.Avatar ?? data.User.Avatar));
        }
        else
        {
            using HttpRequestMessage req = new(HttpMethod.Get, "https://discord.com/api/users/@me");

            req.Headers.Clear();
            req.Headers.Add("Authorization", $"Bearer {accessToken}");

            var res = await client.SendAsync(req);
            var data = await res.Content.ReadAsAsync<DiscordOAuthUserResponseDto>();

            return new User(
                data.Id,
                data.GlobalName ?? data.Username,
                DiscordUtils.GetAvatarUrl(data.Id, data.Discriminator, data.Avatar));
        }
    }
}
