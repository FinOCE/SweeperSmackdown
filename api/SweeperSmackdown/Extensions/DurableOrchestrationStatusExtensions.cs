using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace SweeperSmackdown.Extensions;

public static class DurableOrchestrationStatusExtensions
{
    public static bool IsInactive(this DurableOrchestrationStatus status) =>
        status == null ||
        status.RuntimeStatus != OrchestrationRuntimeStatus.Running;
}
