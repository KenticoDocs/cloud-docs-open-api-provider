using System.Threading.Tasks;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using OpenApiProvider.Configuration;
using OpenApiProvider.Constants;
using OpenApiProvider.Helpers;

namespace OpenApiProvider.Starters
{
    public class ReferenceUpdateStarter
    {
        [FunctionName(Functions.ReferenceUpdateStarter)]
        public static async Task HttpStart(
            [EventGridTrigger] EventGridEvent eventGridEvent,
            [OrchestrationClient] DurableOrchestrationClient starter,
            ILogger log)
        {
            var eventGridData = (dynamic) eventGridEvent.Data;
            var apiReference = (string) eventGridData.webhook.items.ToString();

            await starter.StartNewAsync(
                Functions.Orchestrator,
                apiReference,
                null
            );

            EventGrid.SetupEventGrid(
                EnvironmentVariables.EventGridReferenceUpdatedEndpoint,
                EnvironmentVariables.EventGridReferenceUpdatedKey
            );

            await EventGrid.SendReferenceEvent(apiReference, Events.ReferenceUpdated);
        }
    }
}