namespace OpenApiProvider.Models
{
    internal class BlobCreatedData
    {
        internal string Api { get; set; }
        internal string ClientRequestId { get; set; }
        internal string RequestId { get; set; }
        internal string ETag { get; set; }
        internal string ContentType { get; set; }
        internal int ContentLength { get; set; }
        internal string BlobType { get; set; }
        internal string Url { get; set; }
        internal string Sequencer { get; set; }
        internal StorageDiagnostics StorageDiagnostics { get; set; }
    }

    internal class StorageDiagnostics
    {
        internal string BatchId { get; set; }
    }
}
