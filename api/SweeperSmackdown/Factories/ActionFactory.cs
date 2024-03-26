using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using Microsoft.Azure.WebPubSub.Common;
using Newtonsoft.Json;
using SweeperSmackdown.Assets;
using SweeperSmackdown.DTOs;
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

    public static WebPubSubAction UpdateVoteState(string lobbyId, VoteGroupResponseDto vote) =>
        WebPubSubAction.CreateSendToGroupAction(
            lobbyId,
            MessageFactory.Create(PubSubEvents.VOTE_STATE_UPDATE, "SYSTEM", vote),
            WebPubSubDataType.Json);
    
    public static WebPubSubAction CreateBoard(string userId, string lobbyId, byte[] gameState) =>
        WebPubSubAction.CreateSendToGroupAction(
            lobbyId,
            MessageFactory.Create(PubSubEvents.BOARD_CREATE, userId, Encoding.UTF8.GetString(gameState)),
            WebPubSubDataType.Json);
}
