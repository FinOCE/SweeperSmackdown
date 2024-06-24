using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using Microsoft.Azure.WebPubSub.Common;
using SweeperSmackdown.Assets;
using SweeperSmackdown.DTOs;
using SweeperSmackdown.DTOs.Websocket;
using SweeperSmackdown.Models;
using System;
using System.Text;

namespace SweeperSmackdown.Factories;

public static class ActionFactory
{
    public static WebPubSubAction AddUserToLobby(string userId, string lobbyId) =>
        WebPubSubAction.CreateAddUserToGroupAction(userId, lobbyId);

    public static WebPubSubAction RemoveUserFromLobby(string userId, string lobbyId) =>
        WebPubSubAction.CreateRemoveUserFromGroupAction(userId, lobbyId);

    public static WebPubSubAction UpdateLobby(string lobbyId, LobbyResponseDto lobby) =>
        WebPubSubAction.CreateSendToGroupAction(
            lobbyId,
            MessageFactory.Create(PubSubEvents.LOBBY_UPDATE, "SYSTEM", lobby),
            WebPubSubDataType.Json);
    
    public static WebPubSubAction AddUser(string userId, string lobbyId, Player player) =>
        WebPubSubAction.CreateSendToGroupAction(
            lobbyId,
            MessageFactory.Create(PubSubEvents.USER_JOIN, userId, player),
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

    public static WebPubSubAction CreateBoard(string userId, string lobbyId, byte[] gameState, bool reset) =>
        WebPubSubAction.CreateSendToGroupAction(
            lobbyId,
            MessageFactory.Create(PubSubEvents.BOARD_CREATE, userId, new { gameState = Encoding.UTF8.GetString(gameState), reset }),
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

    public static WebPubSubAction GameStarting(string lobbyId, DateTime expiry) =>
        WebPubSubAction.CreateSendToGroupAction(
            lobbyId,
            MessageFactory.Create(PubSubEvents.GAME_STARTING, "SYSTEM", expiry),
            WebPubSubDataType.Json);

    public static WebPubSubAction GameCelebrationStarting(string lobbyId, DateTime expiry) =>
        WebPubSubAction.CreateSendToGroupAction(
            lobbyId,
            MessageFactory.Create(PubSubEvents.GAME_CELEBRATION_STARTING, "SYSTEM", expiry),
            WebPubSubDataType.Json);

    public static WebPubSubAction DeleteLobby(string lobbyId) =>
        WebPubSubAction.CreateSendToGroupAction(
            lobbyId,
            MessageFactory.Create(PubSubEvents.LOBBY_DELETE, "SYSTEM", ""));
}
