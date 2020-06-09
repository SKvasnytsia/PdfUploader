using System;

namespace PDFUploader.API.Options
{
    public class ConnectionStringOptions
    {
        public Uri CosmosServiceEndpoint { get; set; }
        public string CosmosAccountKey { get; set; }

        public string BlobStorage { get; set; }

        public void Deconstruct(out Uri serviceEndpoint, out string authKey, out string blobStorage)
        {
            serviceEndpoint = CosmosServiceEndpoint;
            authKey = CosmosAccountKey;
            blobStorage = BlobStorage;
        }
    }
}
