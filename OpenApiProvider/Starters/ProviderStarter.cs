using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using OpenApiProvider.Constants;
using OpenApiProvider.Models;

namespace OpenApiProvider.Starters
{
    public static class ProviderStarter
    {
        private const int WaitForCompletionTime = 230;

        [FunctionName(Functions.ProviderStarter)]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")]
            HttpRequestMessage req,
            [OrchestrationClient] DurableOrchestrationClient starter,
            ILogger log)
        {
            var apiReference = ExtractQueryParameter(req, "api");
            var isPreview = ExtractQueryParameter(req, "isPreview");
            var isTest = ExtractQueryParameter(req, "isTest");

            if (isPreview == "true")
            {
                var response = await starter.RunOrchestrator(
                    req,
                    apiReference,
                    new PreprocessorActivityInput
                    {
                        IsPreview = isPreview,
                        IsTest = isTest,
                        Codename = apiReference
                    }
                );

                return response;
            }

            await starter.PurgeInstanceHistoryAsync(apiReference);
            var orchestratorInstance = await starter.GetStatusAsync(apiReference);

            switch (orchestratorInstance?.RuntimeStatus)
            {
                case OrchestrationRuntimeStatus.Pending:
                case OrchestrationRuntimeStatus.Running:
                    return await starter.WaitAndGetOrchestratorResult(req, apiReference);

                case OrchestrationRuntimeStatus.Completed:
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(
                            orchestratorInstance.Output.ToString(),
                            Encoding.UTF8,
                            "application/json"
                        )
                    };

                default:
                    return await starter.RunOrchestrator(
                        req,
                        apiReference,
                        new PreprocessorActivityInput {IsPreview = isPreview, IsTest = isTest, Codename = apiReference}
                    );
            }
        }

        private static string ExtractQueryParameter(HttpRequestMessage req, string parameterName) =>
            HttpUtility
                .ParseQueryString(req.RequestUri.Query)
                .Get(parameterName);

        private static async Task<HttpResponseMessage> RunOrchestrator(
            this DurableOrchestrationClientBase starter,
            HttpRequestMessage req,
            string apiReference,
            PreprocessorActivityInput orchestratorInput
        )
        {
            var orchestratorId = orchestratorInput.IsPreview == "true"
                ? apiReference + "-preview"
                : apiReference;

            await starter.StartNewAsync(
                Functions.Orchestrator,
                orchestratorId,
                orchestratorInput
            );

            return await starter.WaitAndGetOrchestratorResult(req, orchestratorId);
        }

        private static async Task<HttpResponseMessage> WaitAndGetOrchestratorResult(
            this DurableOrchestrationClientBase client,
            HttpRequestMessage request,
            string orchestratorId
        )
            => await client.WaitForCompletionOrCreateCheckStatusResponseAsync(
                request,
                orchestratorId,
                TimeSpan.FromSeconds(WaitForCompletionTime)
            );
    }
}