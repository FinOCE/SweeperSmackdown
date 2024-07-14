using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using Microsoft.Azure.WebPubSub.Common;
using SweeperSmackdown.Assets;
using SweeperSmackdown.DTOs;
using SweeperSmackdown.DTOs.Websocket;
using SweeperSmackdown.Functions.Entities;
using SweeperSmackdown.Functions.Orchestrators;
using SweeperSmackdown.Structures;
using System.Text;

namespace SweeperSmackdown.Factories;

public static class ActionFactory
{
    public static SendToUserAction CreatedLobby(string lobbyId, string userId) =>
        WebPubSubAction.CreateSendToUserAction(
            userId,
            MessageFactory.Create(PubSubEvents.LOBBY_CREATED, "SYSTEM", lobbyId),
            WebPubSubDataType.Json);

    public static SendToGroupAction UpdateLobbyStatus(string lobbyId, LobbyOrchestratorStatus orchestratorStatus) =>
        WebPubSubAction.CreateSendToGroupAction(
            lobbyId,
            MessageFactory.Create(PubSubEvents.LOBBY_STATUS_UPDATE, "SYSTEM", orchestratorStatus),
            WebPubSubDataType.Json);

    public static AddUserToGroupAction AddUserToLobby(string userId, string lobbyId) =>
        WebPubSubAction.CreateAddUserToGroupAction(userId, lobbyId);

    public static RemoveUserFromGroupAction RemoveUserFromLobby(string userId, string lobbyId) =>
        WebPubSubAction.CreateRemoveUserFromGroupAction(userId, lobbyId);

    public static SendToUserAction JoinLobby(string lobbyId, string userId) =>
        WebPubSubAction.CreateSendToUserAction(
            userId,
            MessageFactory.Create(PubSubEvents.LOBBY_JOIN, userId, lobbyId),
            WebPubSubDataType.Json);

    public static SendToUserAction LeaveLobby(string lobbyId, string userId) =>
        WebPubSubAction.CreateSendToUserAction(
            userId,
            MessageFactory.Create(PubSubEvents.LOBBY_LEAVE, userId, lobbyId),
            WebPubSubDataType.Json);

    public static SendToGroupAction AddPlayer(string lobbyId, Player player) =>
        WebPubSubAction.CreateSendToGroupAction(
            lobbyId,
            MessageFactory.Create(PubSubEvents.PLAYER_ADD, player.Id, player),
            WebPubSubDataType.Json);

    public static SendToGroupAction RemovePlayer(string lobbyId, string userId) =>
        WebPubSubAction.CreateSendToGroupAction(
            lobbyId,
            MessageFactory.Create(PubSubEvents.PLAYER_REMOVE, userId, ""),
            WebPubSubDataType.Json);

    public static SendToGroupAction UpdatePlayer(string lobbyId, Player player) =>
        WebPubSubAction.CreateSendToGroupAction(
            lobbyId,
            MessageFactory.Create(PubSubEvents.PLAYER_UPDATE, player.Id, player),
            WebPubSubDataType.Json);

    public static SendToGroupAction UpdateLobbySettings(string lobbyId, GameSettings settings) =>
        WebPubSubAction.CreateSendToGroupAction(
            lobbyId,
            MessageFactory.Create(PubSubEvents.LOBBY_SETTINGS_UPDATE, "SYSTEM", GameSettingsResponse.FromModel(settings)),
            WebPubSubDataType.Json);

    public static SendToUserAction UpdateLobbySettingsFailed(string userId, GameSettings settings) =>

        WebPubSubAction.CreateSendToUserAction(
            userId,
            MessageFactory.Create(PubSubEvents.LOBBY_SETTINGS_UPDATE_FAILED, "SYSTEM", GameSettingsResponse.FromModel(settings)),
            WebPubSubDataType.Json);

    public static SendToGroupAction UpdateConfigureState(string lobbyId, EGameSettingsStateMachineState state) =>
        WebPubSubAction.CreateSendToGroupAction(
            lobbyId,
            MessageFactory.Create(PubSubEvents.LOBBY_STATE_UPDATE, "SYSTEM", state),
            WebPubSubDataType.Json);
    
    public static SendToUserAction UpdateConfigureStateFailed(string userId, EGameSettingsStateMachineState currentState) =>
        WebPubSubAction.CreateSendToUserAction(
            userId,
            MessageFactory.Create(PubSubEvents.LOBBY_STATE_UPDATE_FAILED, "SYSTEM", currentState),
            WebPubSubDataType.Json);

    public static SendToGroupAction UpdateLobbyHost(string lobbyId, string hostId) =>
        WebPubSubAction.CreateSendToGroupAction(
            lobbyId,
            MessageFactory.Create(PubSubEvents.LOBBY_HOST_UPDATE, "SYSTEM", hostId),
            WebPubSubDataType.Json);

    public static SendToGroupAction UpdateLobbyHostManaged(string lobbyId, bool hostManaged) =>
        WebPubSubAction.CreateSendToGroupAction(
            lobbyId,
            MessageFactory.Create(PubSubEvents.LOBBY_HOST_MANAGED_UPDATE, "SYSTEM", hostManaged),
            WebPubSubDataType.Json);

    public static SendToGroupAction CreateBoard(string userId, string lobbyId, byte[] gameState, bool reset) =>
        WebPubSubAction.CreateSendToGroupAction(
            lobbyId,
            MessageFactory.Create(PubSubEvents.BOARD_CREATE, userId, new { gameState = Encoding.UTF8.GetString(gameState), reset }),
            WebPubSubDataType.Json);

    public static SendToGroupAction MakeMove(string userId, string lobbyId, OnMoveData move) =>
        WebPubSubAction.CreateSendToGroupAction(
            lobbyId,
            MessageFactory.Create(PubSubEvents.MOVE_ADD, userId, move),
            WebPubSubDataType.Json);

    public static SendToUserAction RejectMove(string userId, OnMoveData move) =>
        WebPubSubAction.CreateSendToUserAction(
            userId,
            MessageFactory.Create(PubSubEvents.MOVE_REJECT, userId, move),
            WebPubSubDataType.Json);

    public static SendToGroupAction UpdatePlayerState(string userId, string lobbyId, PlayerState playerState) =>
        WebPubSubAction.CreateSendToGroupAction(
            lobbyId,
            MessageFactory.Create(PubSubEvents.PLAYER_STATE_UPDATE, userId, playerState),
            WebPubSubDataType.Json);

    public static SendToGroupAction GameWon(string userId, string lobbyId) =>
        WebPubSubAction.CreateSendToGroupAction(
            lobbyId,
            MessageFactory.Create(PubSubEvents.GAME_WON, userId, ""),
            WebPubSubDataType.Json);
}
