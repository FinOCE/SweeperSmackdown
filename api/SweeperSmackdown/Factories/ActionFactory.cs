using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using Microsoft.Azure.WebPubSub.Common;
using SweeperSmackdown.Assets;
using SweeperSmackdown.DTOs;

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
            MessageFactory.Create(PubSubEvents.LOBBY_UPDATE, userId, lobby, null),
            WebPubSubDataType.Json);
    
    public static WebPubSubAction AddUser(string userId, string lobbyId) =>
        WebPubSubAction.CreateSendToGroupAction(
            lobbyId,
            MessageFactory.Create(PubSubEvents.USER_JOIN, userId, userId, $"<@{userId}> has joined the lobby"),
            WebPubSubDataType.Json);

    public static WebPubSubAction UpdateUser(string userId, string lobbyId) =>
        WebPubSubAction.CreateSendToGroupAction(
            lobbyId,
            MessageFactory.Create(PubSubEvents.USER_UPDATE, userId, userId, null),
            WebPubSubDataType.Json);

    // TODO: Implement users beyond just userId in order to implement above meaningfully

    public static WebPubSubAction RemoveUser(string userId, string lobbyId) =>
        WebPubSubAction.CreateSendToGroupAction(
            lobbyId,
            MessageFactory.Create(PubSubEvents.USER_LEAVE, userId, userId, $"<@{userId}> has left the lobby"),
            WebPubSubDataType.Json);

    public static WebPubSubAction AddVote(string userId, string lobbyId, string choice) =>
        WebPubSubAction.CreateSendToGroupAction(
            lobbyId,
            MessageFactory.Create(PubSubEvents.VOTE_ADD, userId, choice, null),
            WebPubSubDataType.Json);

    public static WebPubSubAction RemoveVote(string userId, string lobbyId, string choice) =>
        WebPubSubAction.CreateSendToGroupAction(
            lobbyId,
            MessageFactory.Create(PubSubEvents.VOTE_REMOVE, userId, choice, null),
            WebPubSubDataType.Json);

    public static WebPubSubAction UpdateVoteRequirement(string userId, string lobbyId, int votesRequired) =>
        WebPubSubAction.CreateSendToGroupAction(
            lobbyId,
            MessageFactory.Create(PubSubEvents.VOTE_UPDATE_REQUIREMENT, userId, votesRequired, null),
            WebPubSubDataType.Json);

    public static WebPubSubAction CreateBoard(string userId, string lobbyId) =>
        WebPubSubAction.CreateSendToGroupAction(
            lobbyId,
            MessageFactory.Create(PubSubEvents.BOARD_CREATE, userId, userId, null),
            WebPubSubDataType.Json);
}
