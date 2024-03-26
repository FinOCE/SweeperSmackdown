using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using Microsoft.Azure.WebJobs;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace SweeperSmackdown.Functions.Http;

public static class NegotiateFunction
{
    [FunctionName(nameof(NegotiateFunction))]
    public static IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "negotiate")] HttpRequest req,
        [WebPubSubConnection(Hub = PubSubConstants.HUB_NAME, UserId = "{query.userId}")] WebPubSubConnection connection)
    {
        // Only allow if user is logged in
        var requesterId = req.GetUserId();

        if (requesterId == null)
            return new StatusCodeResult(401);

        // Check if the query and authorization user ID match
        if (requesterId != req.Query["userId"])
            return new BadRequestObjectResult(
                new string[]
                {
                    "The 'userId' query parameter does not match your user ID"
                });

        // Respond to request
        return new OkObjectResult(connection);
    }
}
