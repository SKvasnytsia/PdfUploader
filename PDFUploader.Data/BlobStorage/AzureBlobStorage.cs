using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using PDFUploader.Domain.Entities;
using PDFUploader.Domain.Interfaces.Blobs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PDFUploader.Data.BlobStorage
{
    public class AzureBlobStorage : IBlobStorage
    {
        private const string CONTAINER_REFERENCE = "pdfs";

        private readonly CloudBlobClient _cloudBlobClient;
        public AzureBlobStorage(string blobStorageConnection)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(blobStorageConnection);

            _cloudBlobClient = storageAccount.CreateCloudBlobClient();

        }

        async Task<string> IBlobStorage.UploadAsync(string filename, Stream content)
        {
            CloudBlobContainer container = _cloudBlobClient.GetContainerReference(CONTAINER_REFERENCE);

            await container.CreateIfNotExistsAsync();

            // The FileName property should only be used for display purposes.
            var pdfBlob = container.GetBlockBlobReference(filename);

            await pdfBlob.UploadFromStreamAsync(content);

            return pdfBlob.Uri.ToString();
        }

        async Task<PdfMetadata[]> IBlobStorage.GetFileCollectionAsync()
        {
            CloudBlobContainer container = _cloudBlobClient.GetContainerReference(CONTAINER_REFERENCE);

            await container.CreateIfNotExistsAsync();

            BlobResultSegment resultSegment = await container.ListBlobsSegmentedAsync(null);
            return GetBlobCollection(resultSegment).ToArray();
        }

        private IEnumerable<PdfMetadata> GetBlobCollection(BlobResultSegment resultSegment)
        {
            foreach (IListBlobItem item in resultSegment.Results)
            {
                if (item.GetType() == typeof(CloudBlockBlob))
                {
                    CloudBlockBlob blob = (CloudBlockBlob)item;
                    yield return new PdfMetadata
                    {
                        Name = blob.Name,
                        FileSize = blob.Properties.Length.ToString(),
                        Location = blob.Uri.ToString()
                    };
                }

            }
        } 

        async Task<string> IBlobStorage.GetDownloadLinkAsync(string fileName)
        {
            CloudBlobContainer container = _cloudBlobClient.GetContainerReference(CONTAINER_REFERENCE);
            
            await container.CreateIfNotExistsAsync();
            
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

            if (await blockBlob.ExistsAsync())
            {
                //Create an ad-hoc Shared Access Policy with read permissions which will expire in 12 hours
                SharedAccessBlobPolicy policy = new SharedAccessBlobPolicy()
                {
                    Permissions = SharedAccessBlobPermissions.Read,
                    SharedAccessExpiryTime = DateTime.UtcNow.AddHours(12),
                };
                //Set content-disposition header for force download
                SharedAccessBlobHeaders headers = new SharedAccessBlobHeaders()
                {
                    ContentDisposition = string.Format("attachment;filename=\"{0}\"", fileName),
                };
                var sasToken = blockBlob.GetSharedAccessSignature(policy, headers);
                return blockBlob.Uri.AbsoluteUri + sasToken;
            }
            return null;
        }

        async Task<bool> IBlobStorage.DeleteAsync(string fileName)
        {
            CloudBlobContainer container = _cloudBlobClient.GetContainerReference(CONTAINER_REFERENCE);
            
            await container.CreateIfNotExistsAsync();

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);
            return await blockBlob.DeleteIfExistsAsync();
        }
    }
}
