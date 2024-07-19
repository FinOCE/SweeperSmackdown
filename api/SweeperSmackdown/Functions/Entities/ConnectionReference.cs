using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using SweeperSmackdown.Functions.Orchestrators;
using SweeperSmackdown.Structures;
using SweeperSmackdown.Utils;

namespace SweeperSmackdown.Functions.Entities;

/// <summary>
/// An entity to reference a user's connection to their lobby. Used to quickly find the lobby associated with a
/// connection event.
/// </summary>
public interface IConnectionReference
{
    /// <summary>
    /// Create a new connection reference for the specified user ID.
    /// </summary>
    /// <param name="userId">The ID of the user</param>
    public void Create();

    /// <summary>
    /// Delete the entity.
    /// </summary>
    public void Delete();

    /// <summary>
    /// Set the lobby ID the connection is associated with. If an empty string is provided, it sets the value to null.
    /// This is done because of the way durable entity signals work.
    /// </summary>
    /// <param name="lobbyId">The lobby ID the connection is associated with</param>
    public void SetLobbyId(string lobbyId);
}

[DataContract]
public class ConnectionReference : IConnectionReference
{
    [DataMember]
    public string? LobbyId { get; set; }

    private string UserId =>
        Entity.Current.EntityId.EntityKey;

    [FunctionName(nameof(ConnectionReference))]
    public static Task Run([EntityTrigger] IDurableEntityContext ctx) =>
        ctx.DispatchAsync<ConnectionReference>();

    public void Create() =>
        LobbyId = null;

    public void Delete() =>
        Entity.Current.DeleteState();

    public void SetLobbyId(string lobbyId) =>
        LobbyId = lobbyId == "" ? null : lobbyId;
}
