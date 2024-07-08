using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Guide_Me.Services
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly ILogger<BlobStorageService> _logger;

        public BlobStorageService(IConfiguration configuration, ILogger<BlobStorageService> logger)
        {
            var connectionString = configuration.GetConnectionString("AzureStorage");
            _blobServiceClient = new BlobServiceClient(connectionString);
            _logger = logger;
        }

        public string GetBlobUrl(string containerName, string blobName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);
            return blobClient.Uri.ToString();
        }

        public string GetBlobUrlmedia(string MediaContent)
        {
            // Replace this with actual container and blob name based on your storage structure
            string containerName = "firstcontainer";
            // Assuming MediaContent is the blob name or a unique identifier for the blob
            string blobName = MediaContent;
            // Call BlobStorageService to get the blob URL
            return GetBlobUrl(containerName, blobName);
        }

        public async Task DownloadBlobAsync(string blobName, string downloadFilePath)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient("firstcontainer");
            var blobClient = containerClient.GetBlobClient(blobName);

            // Ensure the directory exists before attempting to download
            var directory = Path.GetDirectoryName(downloadFilePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            try
            {
                await blobClient.DownloadToAsync(downloadFilePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error downloading blob '{blobName}' to '{downloadFilePath}': {ex.Message}");
                throw; // Optionally handle or throw the exception as per your application's error handling strategy
            }
        }

        public async Task<string> UploadBlobAsync(string containerName, string blobName, Stream content, string contentType)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                var blobClient = containerClient.GetBlobClient(blobName);

                await blobClient.UploadAsync(content, new BlobHttpHeaders { ContentType = contentType });

                return blobClient.Uri.ToString();
            }
            catch (RequestFailedException ex)
            {
                _logger.LogError(ex, $"Request failed uploading blob '{blobName}' to container '{containerName}': Status={ex.Status}, ErrorCode={ex.ErrorCode}");
                throw new Exception($"Failed to upload blob '{blobName}' to container '{containerName}': {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error uploading blob '{blobName}' to container '{containerName}': {ex.Message}");
                throw; // Optionally handle or throw the exception as per your application's error handling strategy
            }
        }

    }
}
