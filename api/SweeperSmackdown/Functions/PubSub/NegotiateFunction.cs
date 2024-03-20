using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using Microsoft.Azure.WebJobs;
using SweeperSmackdown.Assets;

namespace SweeperSmackdown.Functions.PubSub;

public static class NegotiateFunction
{
    [FunctionName(nameof(NegotiateFunction))]
    public static WebPubSubConnection Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "negotiate")] HttpRequest req,
        [WebPubSubConnection(Hub = PubSubConstants.HUB_NAME, UserId = "{query.userId}")] WebPubSubConnection connection)
    {
        // TODO: Add validation to ensure query.userId matches the requester's authorized account
        
        return connection;
    }
}
