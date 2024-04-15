using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using Microsoft.Azure.WebPubSub.Common;
using SweeperSmackdown.Assets;
using SweeperSmackdown.DTOs;
using SweeperSmackdown.DTOs.Websocket;
using SweeperSmackdown.Extensions;
using System;
using System.Text;

namespace SweeperSmackdown.Factories;

public static class ActionFactory
{
    public static WebPubSubAction AddUserToLobby(string userId, string lobbyId) =>
        WebPubSubAction.CreateAddUserToGroupAction(userId, lobbyId);

    public static WebPubSubAction RemoveUserFromLobby(string userId, string lobbyId) =>
        WebPubSubAction.CreateRemoveUserFromGroupAction(userId, lobbyId);

    public static WebPubSubAction UpdateLobby(string userId, string lobbyId, LobbyResponseDto lobby) =>
        WebPubSubAction.CreateSendToGroupAction(
            lobbyId,
            MessageFactory.Create(PubSubEvents.LOBBY_UPDATE, userId, lobby),
            WebPubSubDataType.Json);
    
    public static WebPubSubAction AddUser(string userId, string lobbyId) =>
        WebPubSubAction.CreateSendToGroupAction(
            lobbyId,
            MessageFactory.Create(PubSubEvents.USER_JOIN, userId, ""),
            WebPubSubDataType.Json);

    public static WebPubSubAction RemoveUser(string userId, string lobbyId) =>
        WebPubSubAction.CreateSendToGroupAction(
            lobbyId,
            MessageFactory.Create(PubSubEvents.USER_LEAVE, userId, ""),
            WebPubSubDataType.Json);

    public static WebPubSubAction StartLobby(string lobbyId) =>
        WebPubSubAction.CreateSendToGroupAction(
            lobbyId,
            MessageFactory.Create(PubSubEvents.LOBBY_START, "SYSTEM", ""),
            WebPubSubDataType.Json);

    public static WebPubSubAction UpdateVoteState(string lobbyId, VoteGroupResponseDto vote) =>
        WebPubSubAction.CreateSendToGroupAction(
            lobbyId,
            MessageFactory.Create(PubSubEvents.VOTE_STATE_UPDATE, "SYSTEM", vote),
            WebPubSubDataType.Json);

    public static WebPubSubAction StartTimer(string lobbyId, DateTime expiry) =>
        WebPubSubAction.CreateSendToGroupAction(
            lobbyId,
            MessageFactory.Create(PubSubEvents.TIMER_START, "SYSTEM", new { expiry = expiry.ToUnixTimeMilliseconds() }),
            WebPubSubDataType.Json);
    
    public static WebPubSubAction ResetTimer(string lobbyId) =>
        WebPubSubAction.CreateSendToGroupAction(
            lobbyId,
            MessageFactory.Create(PubSubEvents.TIMER_RESET, "SYSTEM", ""),
            WebPubSubDataType.Json);

    public static WebPubSubAction CreateBoard(string userId, string lobbyId, byte[] gameState) =>
        WebPubSubAction.CreateSendToGroupAction(
            lobbyId,
            MessageFactory.Create(PubSubEvents.BOARD_CREATE, userId, Encoding.UTF8.GetString(gameState)),
            WebPubSubDataType.Json);

    public static WebPubSubAction MakeMove(string userId, string lobbyId, OnMoveData move) =>
        WebPubSubAction.CreateSendToGroupAction(
            lobbyId,
            MessageFactory.Create(PubSubEvents.MOVE_ADD, userId, move),
            WebPubSubDataType.Json);

    public static WebPubSubAction GameWon(string userId, string lobbyId) =>
        WebPubSubAction.CreateSendToGroupAction(
            lobbyId,
            MessageFactory.Create(PubSubEvents.GAME_WON, userId, ""),
            WebPubSubDataType.Json);
}
