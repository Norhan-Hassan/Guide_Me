using System.Threading.Tasks;

namespace Guide_Me.Services
{
    public interface IBlobStorageService
    {
        string GetBlobUrl(string containerName, string blobName);
        string GetBlobUrlmedia(string MediaContent);
        Task DownloadBlobAsync(string blobName, string downloadFilePath);
    }
}
