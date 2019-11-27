using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using OpenApiProvider.Constants;
using OpenApiProvider.Models;

namespace OpenApiProvider.Orchestrators
{
    public static class Orchestrator
    {
        [FunctionName(Functions.Orchestrator)]
        public static async Task<string> RunOrchestrator([OrchestrationTrigger] DurableOrchestrationContext context)
        {
            var orchestratorInput = context.GetInput<PreprocessorActivityInput>();

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

            var blobUrl = await context.WaitForExternalEvent<string>(Events.BlobCreated);

            var blobContent = await context.CallActivityAsync<string>(
                Functions.GetBlobFromStorageActivity,
                blobUrl
            );

            return blobContent;
        }
    }
}