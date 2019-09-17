using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using OpenApiProvider.Configuration;
using OpenApiProvider.Constants;

namespace OpenApiProvider.Activities
{
    public static class GetBlobFromStorage
    {
        [FunctionName(Functions.GetBlobFromStorageActivity)]
        public static async Task<string> GetBlob([ActivityTrigger] string blobUrl, ILogger log)
        {
            var cloudStorageAccount =
                CloudStorageAccount.Parse(EnvironmentVariables.AzureConnectionString);
            var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            var cloudBlockBlob = new CloudBlockBlob(
                new Uri(blobUrl),
                cloudBlobClient
            );

            return await cloudBlockBlob.DownloadTextAsync();
        }
    }
}