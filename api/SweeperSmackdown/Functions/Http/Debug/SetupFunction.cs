using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SweeperSmackdown.Extensions;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Http;

public static class SetupFunction
{
#if DEBUG
    [FunctionName(nameof(SetupFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "debug/setup")] HttpRequest req,
        [CosmosDB(Connection = "CosmosDbConnectionString")] CosmosClient cosmosClient)
    {
        await cosmosClient.RegenerateContainers();
        return new NoContentResult();
    }
#endif
}
