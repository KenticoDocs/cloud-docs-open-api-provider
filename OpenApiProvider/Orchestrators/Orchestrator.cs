using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using OpenApiProvider.Constants;
using OpenApiProvider.Models;

namespace OpenApiProvider.Orchestrators
{
    public static class Orchestrator
    {
        [FunctionName(Functions.Orchestrator)]
        public static async Task<string> RunOrchestrator([OrchestrationTrigger] DurableOrchestrationContext context, ILogger logger)
        {
            var orchestratorInput = context.GetInput<PreprocessorActivityInput>();
            logger.Log(LogLevel.Trace, $"Running orchestrator. Input: {(orchestratorInput == null ? "set" : "unset")}. Instance id: {context.InstanceId}");

            if (orchestratorInput != null)
            {
                await context.CallActivityAsync(
                    Functions.TriggerPreprocessorActivity,
                    new PreprocessorActivityInput
                    {
                        Codename = orchestratorInput.Codename,
                        IsTest = orchestratorInput.IsTest,
                        IsPreview = orchestratorInput.IsPreview
                    }
                );
            }

            logger.Log(LogLevel.Trace, $"Orchestrator waiting. Input: {(orchestratorInput == null ? "set" : "unset")}. Instance id: {context.InstanceId}");
            var blobUrl = await context.WaitForExternalEvent<string>(Events.BlobCreated);
            logger.Log(LogLevel.Trace, $"Orchestrator got blob");

            var blobContent = await context.CallActivityAsync<string>(
                Functions.GetBlobFromStorageActivity,
                blobUrl
            );

            return blobContent;
        }
    }
}