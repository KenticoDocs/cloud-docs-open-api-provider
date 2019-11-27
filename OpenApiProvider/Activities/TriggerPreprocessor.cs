using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using OpenApiProvider.Configuration;
using OpenApiProvider.Constants;
using OpenApiProvider.Helpers;
using OpenApiProvider.Models;

namespace OpenApiProvider.Activities
{
    public static class TriggerPreprocessor
    {
        [FunctionName(Functions.TriggerPreprocessorActivity)]
        public static async Task Trigger([ActivityTrigger] PreprocessorActivityInput input, ILogger log)
        {
            var eventGrid = new EventGrid(
                EnvironmentVariables.EventGridReferenceRequestedEndpoint,
                EnvironmentVariables.EventGridReferenceRequestedKey
                );

            var eventType = input.IsPreview == "true"
                ? Events.ReferencePreview
                : Events.ReferenceInitialize;

            await eventGrid.SendReferenceEvent(input.Codename, eventType, input.IsTest);
        }
    }
}