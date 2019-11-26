using System;
using System.Linq;
using System.Threading;
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
    public static class BlobCreatedStarter
    {
        public const short NumberOfRetries = 100;

        [FunctionName(Functions.BlobCreatedStarter)]
        public static async Task EventGridStart(
            [EventGridTrigger] EventGridEvent eventGridEvent,
            [OrchestrationClient] DurableOrchestrationClient starter,
            ILogger log)
        {
            var eventGridData = (dynamic)eventGridEvent.Data;
            var blobUrl = (string)eventGridData.url;
            var instanceId = blobUrl
                .Split("/")
                .Last()
                .Split(".")
                .First();

            await starter.RaiseEventWithRetryAsync(instanceId, blobUrl);

            if (!blobUrl.Contains("preview"))
            {
                EventGrid.SetupEventGrid(
                    EnvironmentVariables.EventGridReferenceUpdatedEndpoint,
                    EnvironmentVariables.EventGridReferenceUpdatedKey
                );

                await EventGrid.SendReferenceEvent(instanceId, Events.ReferenceUpdated);
            }
        }

        private static async Task RaiseEventWithRetryAsync(this DurableOrchestrationClient starter, string instanceId, string blobUrl)
        {
            for (var i = 1; i <= NumberOfRetries; i++)
            {
                try
                {
                    await starter.RaiseEventAsync(instanceId, Events.BlobCreated, blobUrl);

                    return;
                }
                catch (InvalidOperationException)
                {
                    if (i == NumberOfRetries)
                    {
                        throw;
                    }

                    Thread.Sleep(1000);
                }
            }
        }
    }
}