using System;

namespace OpenApiProvider.Configuration
{
    internal static class EnvironmentVariables
    {
        internal static string AzureConnectionString => 
            Environment.GetEnvironmentVariable("Storage.ConnectionString");
        internal static string EventGridReferenceRequestedEndpoint =>
            Environment.GetEnvironmentVariable("EventGrid.ReferenceRequested.Endpoint");

        internal static string EventGridReferenceRequestedKey =>
            Environment.GetEnvironmentVariable("EventGrid.ReferenceRequested.Key");

        internal static string EventGridReferenceUpdatedEndpoint =>
            Environment.GetEnvironmentVariable("EventGrid.ReferenceUpdated.Endpoint");

        internal static string EventGridReferenceUpdatedKey =>
            Environment.GetEnvironmentVariable("EventGrid.ReferenceUpdated.Key");
    }
}