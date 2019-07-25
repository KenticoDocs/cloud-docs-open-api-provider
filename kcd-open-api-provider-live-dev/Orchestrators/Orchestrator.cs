using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenApiProvider.Constants;
using OpenApiProvider.Models;

namespace OpenApiProvider.Orchestrators
{
    public static class Orchestrator
    {
        [FunctionName(Functions.Orchestrator)]
        public static async Task<JObject> RunOrchestrator([OrchestrationTrigger] DurableOrchestrationContext context)
        {
            var isTest = context.GetInput<string>();
            await context.CallActivityAsync(
                Functions.TriggerPreprocessorActivity,
                new PreprocessorActivityInput {Codename = context.InstanceId, IsTest = isTest}
            );

            var blobUrl = await context.WaitForExternalEvent<string>(Events.BlobCreated);
            var blobContent = await context.CallActivityAsync<string>(Functions.GetBlobFromStorageActivity, blobUrl);

            var blobJson = (JObject) JsonConvert.DeserializeObject(blobContent);

            return blobJson;
        }
    }
}