using Microsoft.Azure.Functions.Worker;
using SweeperSmackdown.Stats.Assets;
using SweeperSmackdown.Stats.DTOs;
using SweeperSmackdown.Stats.Models;

namespace SweeperSmackdown.Stats.Functions.ServiceBus;

public static class StatServiceBusFunction
{
    [Function(nameof(StatServiceBusFunction))]
    [CosmosDBOutput(Constants.DATABASE_NAME, Constants.STATS_CONTAINER_NAME)]
    public static PlayerInfo Run(
        [ServiceBusTrigger(Constants.SERVICE_BUS_QUEUE_NAME)] StatRequest statRequest,
        [CosmosDBInput(Constants.DATABASE_NAME, Constants.STATS_CONTAINER_NAME, Id = "{Subject}", PartitionKey = "{Subject}")] PlayerInfo? playerInfo) =>
        statRequest.ApplyToModel(playerInfo);
}
