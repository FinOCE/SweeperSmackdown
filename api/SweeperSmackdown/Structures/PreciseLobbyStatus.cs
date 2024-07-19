using Newtonsoft.Json;
using SweeperSmackdown.Functions.Entities;
using SweeperSmackdown.Functions.Orchestrators;
using System;

namespace SweeperSmackdown.Structures;

public class PreciseLobbyStatus
{
    [JsonProperty("status")]
    public ELobbyStatus Status { get; set; }

    [JsonProperty("statusUntil")]
    public DateTime? StatusUntil { get; set; }

    [JsonProperty("configureState")]
    public EGameSettingsStateMachineState? ConfigureState { get; set; }

    public PreciseLobbyStatus(
        ELobbyStatus status,
        DateTime? statusUntil)
    {
        Status = status;
        StatusUntil = statusUntil;
    }

    [JsonConstructor]
    public PreciseLobbyStatus(
        ELobbyStatus status,
        DateTime? statusUntil,
        EGameSettingsStateMachineState? configureState)
    {
        Status = status;
        StatusUntil = statusUntil;
        ConfigureState = configureState;
    }

    public PreciseLobbyStatus(
        LobbyOrchestratorStatus lobbyOrchestratorStatus,
        EGameSettingsStateMachineState? configureState)
    {
        Status = lobbyOrchestratorStatus.Status;
        StatusUntil = lobbyOrchestratorStatus.StatusUntil;
        ConfigureState = configureState;
    }
}
