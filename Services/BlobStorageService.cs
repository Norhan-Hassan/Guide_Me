using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using Guide_Me.Models;

namespace Guide_Me.Services
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;

        public BlobStorageService(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("AzureStorage");
            _blobServiceClient = new BlobServiceClient(connectionString);
        }
        private string GetBlobUrl(string containerName, string blobName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);
            return blobClient.Uri.ToString();
        }
        public string GetBlobUrlmedia(string MediaContent)
        {
            // Replace this with actual container and blob name based on your storage structure
            string containerName = "firstcontainer";
            // Assuming CityImage is the blob name or a unique identifier for the blob
            string blobName = MediaContent;
            // Call BlobStorageService to get the blob URL
            return GetBlobUrl(containerName, blobName);
        }
    }
}
