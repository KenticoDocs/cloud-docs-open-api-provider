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
            EventGrid.SetupEventGrid(
                EnvironmentVariables.EventGridReferenceRequestedEndpoint,
                EnvironmentVariables.EventGridReferenceRequestedKey
            );

            if (input.Codename.Contains("preview"))
            {
                await EventGrid.SendReferenceEvent(input.Codename, Events.ReferencePreview, input.IsTest);
            }
            else
            {
                await EventGrid.SendReferenceEvent(input.Codename, Events.ReferenceInitialize, input.IsTest);
            }
        }
    }
}