using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using OpenApiProvider.Constants;

namespace OpenApiProvider.Starters
{
    public static class BlobCreatedStarter
    {
        [FunctionName(Functions.BlobCreatedStarter)]
        public static async Task EventGridStart(
            [EventGridTrigger] EventGridEvent eventGridEvent,
            [OrchestrationClient] DurableOrchestrationClient starter,
            ILogger log)
        {
            var eventGridData = (dynamic) eventGridEvent.Data;
            var blobUrl = (string)eventGridData.url;
            var instanceId = blobUrl
                .Split("/")
                .Last();

            await starter.RaiseEventAsync(instanceId, Events.BlobCreated, blobUrl);
        }
    }
}