using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using OpenApiProvider.Constants;

namespace OpenApiProvider.Starters
{
    public static class ProviderStarter
    {
        private const int WaitForCompletionTime = 220;

        [FunctionName(Functions.ProviderStarter)]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")]
            HttpRequestMessage req,
            [OrchestrationClient] DurableOrchestrationClient starter,
            ILogger log)
        {
            var apiReference = HttpUtility
                .ParseQueryString(req.RequestUri.Query)
                .Get("api");
            var isTest = HttpUtility
                .ParseQueryString(req.RequestUri.Query)
                .Get("isTest");

            if (apiReference.Contains("preview"))
            {
                return await starter.RunOrchestrator(req, apiReference, isTest);
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
                            "application/json"
                        )
                    };

                default:
                    return await starter.RunOrchestrator(req, apiReference, isTest);
            }
        }

        private static async Task<HttpResponseMessage> RunOrchestrator(
            this DurableOrchestrationClientBase starter,
            HttpRequestMessage req,
            string apiReference,
            string isTest
        )
        {
            await starter.StartNewAsync(
                Functions.Orchestrator,
                apiReference,
                isTest
            );

            return await starter.WaitAndGetOrchestratorResult(req, apiReference);
        }

        private static async Task<HttpResponseMessage> WaitAndGetOrchestratorResult(
            this DurableOrchestrationClientBase client,
            HttpRequestMessage request,
            string apiReference
        ) =>
            await client.WaitForCompletionOrCreateCheckStatusResponseAsync(
                request,
                apiReference,
                TimeSpan.FromSeconds(WaitForCompletionTime)
            );
    }
}