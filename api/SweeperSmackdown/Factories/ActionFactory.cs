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
    public static WebPubSubAction CreatedLobby(string lobbyId, string userId) =>
        WebPubSubAction.CreateSendToUserAction(
            userId,
            MessageFactory.Create(PubSubEvents.LOBBY_CREATED, "SYSTEM", lobbyId),
            WebPubSubDataType.Json);

    public static WebPubSubAction UpdateLobbyStatus(string lobbyId, LobbyOrchestratorStatus orchestratorStatus) =>
        WebPubSubAction.CreateSendToGroupAction(
            lobbyId,
            MessageFactory.Create(PubSubEvents.LOBBY_STATUS_UPDATE, "SYSTEM", orchestratorStatus),
            WebPubSubDataType.Json);

    public static WebPubSubAction AddUserToLobby(string userId, string lobbyId) =>
        WebPubSubAction.CreateAddUserToGroupAction(userId, lobbyId);

    public static WebPubSubAction RemoveUserFromLobby(string userId, string lobbyId) =>
        WebPubSubAction.CreateRemoveUserFromGroupAction(userId, lobbyId);

    public static WebPubSubAction JoinLobby(string lobbyId, string userId) =>
        WebPubSubAction.CreateSendToUserAction(
            userId,
            MessageFactory.Create(PubSubEvents.LOBBY_JOIN, userId, lobbyId),
            WebPubSubDataType.Json);

    public static WebPubSubAction LeaveLobby(string lobbyId, string userId) =>
        WebPubSubAction.CreateSendToUserAction(
            userId,
            MessageFactory.Create(PubSubEvents.LOBBY_LEAVE, userId, lobbyId),
            WebPubSubDataType.Json);

    public static WebPubSubAction AddPlayer(string lobbyId, Player player) =>
        WebPubSubAction.CreateSendToGroupAction(
            lobbyId,
            MessageFactory.Create(PubSubEvents.PLAYER_ADD, player.Id, player),
            WebPubSubDataType.Json);

    public static WebPubSubAction RemovePlayer(string lobbyId, string userId) =>
        WebPubSubAction.CreateSendToGroupAction(
            lobbyId,
            MessageFactory.Create(PubSubEvents.PLAYER_REMOVE, userId, ""),
            WebPubSubDataType.Json);

    public static WebPubSubAction UpdatePlayer(string lobbyId, Player player) =>
        WebPubSubAction.CreateSendToGroupAction(
            lobbyId,
            MessageFactory.Create(PubSubEvents.PLAYER_UPDATE, player.Id, player),
            WebPubSubDataType.Json);

    public static WebPubSubAction UpdateLobby(string lobbyId, LobbyResponseDto lobby) =>
        WebPubSubAction.CreateSendToGroupAction(
            lobbyId,
            MessageFactory.Create(PubSubEvents.LOBBY_UPDATE, "SYSTEM", lobby),
            WebPubSubDataType.Json);

    public static WebPubSubAction UpdateLobbySettings(string lobbyId, GameSettings settings) =>
        WebPubSubAction.CreateSendToGroupAction(
            lobbyId,
            MessageFactory.Create(PubSubEvents.LOBBY_SETTINGS_UPDATE, "SYSTEM", settings), // TODO: Create settings response dto
            WebPubSubDataType.Json);

    public static WebPubSubAction UpdateLobbySettingsFailed(string userId, GameSettings settings) =>

        WebPubSubAction.CreateSendToUserAction(
            userId,
            MessageFactory.Create(PubSubEvents.LOBBY_SETTINGS_UPDATE_FAILED, "SYSTEM", settings), // TODO: Create settings response dto
            WebPubSubDataType.Json);

    public static WebPubSubAction UpdateConfigureState(string lobbyId, EGameSettingsStateMachineState state) =>
        WebPubSubAction.CreateSendToGroupAction(
            lobbyId,
            MessageFactory.Create(PubSubEvents.LOBBY_STATE_UPDATE, "SYSTEM", state),
            WebPubSubDataType.Json);
    
    public static WebPubSubAction UpdateConfigureStateFailed(string userId, EGameSettingsStateMachineState currentState) =>
        WebPubSubAction.CreateSendToUserAction(
            userId,
            MessageFactory.Create(PubSubEvents.LOBBY_STATE_UPDATE_FAILED, "SYSTEM", currentState),
            WebPubSubDataType.Json);

    public static WebPubSubAction UpdateLobbyHost(string lobbyId, string hostId) =>
        WebPubSubAction.CreateSendToGroupAction(
            lobbyId,
            MessageFactory.Create(PubSubEvents.LOBBY_HOST_UPDATE, "SYSTEM", hostId),
            WebPubSubDataType.Json);

    public static WebPubSubAction UpdateLobbyHostManaged(string lobbyId, bool hostManaged) =>
        WebPubSubAction.CreateSendToGroupAction(
            lobbyId,
            MessageFactory.Create(PubSubEvents.LOBBY_HOST_MANAGED_UPDATE, "SYSTEM", hostManaged),
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

    public static WebPubSubAction RejectMove(string userId, OnMoveData move) =>
        WebPubSubAction.CreateSendToUserAction(
            userId,
            MessageFactory.Create(PubSubEvents.MOVE_REJECT, userId, move),
            WebPubSubDataType.Json);

    public static WebPubSubAction UpdatePlayerState(string userId, string lobbyId, PlayerState playerState) =>
        WebPubSubAction.CreateSendToGroupAction(
            lobbyId,
            MessageFactory.Create(PubSubEvents.PLAYER_STATE_UPDATE, userId, playerState),
            WebPubSubDataType.Json);

    public static WebPubSubAction GameWon(string userId, string lobbyId) =>
        WebPubSubAction.CreateSendToGroupAction(
            lobbyId,
            MessageFactory.Create(PubSubEvents.GAME_WON, userId, ""),
            WebPubSubDataType.Json);
}
