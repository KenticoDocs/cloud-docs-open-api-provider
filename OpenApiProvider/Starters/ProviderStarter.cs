using System;
using System.Net;
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
        private const int WaitForCompletionTime = 360;

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

            if (apiReference == null)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

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
                            "text/html"
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
            HttpRequestMessage request,
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

            return await starter.WaitAndGetOrchestratorResult(request, orchestratorId);
        }

        private static async Task<HttpResponseMessage> WaitAndGetOrchestratorResult(
            this DurableOrchestrationClientBase client,
            HttpRequestMessage request,
            string orchestratorId
        )
        {
            var orchestratorResponse = await client.WaitForCompletionOrCreateCheckStatusResponseAsync(
                request,
                orchestratorId,
                TimeSpan.FromSeconds(WaitForCompletionTime)
            );

            var orchestratorInstance = await client.GetStatusAsync(orchestratorId);

            if (orchestratorInstance.RuntimeStatus != OrchestrationRuntimeStatus.Completed)
            {
                await client.PurgeInstanceHistoryAsync(orchestratorId);

                return GetTimeoutErrorMessage();
            }

            var responseContent = await orchestratorResponse.Content.ReadAsAsync<string>();
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseContent, Encoding.UTF8, "text/html")
            };

            return response;
        }

        private static HttpResponseMessage GetTimeoutErrorMessage() =>
            new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(
                    "API reference generation timed out.",
                    Encoding.UTF8,
                    "text/html"
                )
            };
    }
}
