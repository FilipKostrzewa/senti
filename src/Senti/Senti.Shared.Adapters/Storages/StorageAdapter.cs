using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Senti.Shared.Models;
using System.Text;

namespace Senti.Shared.Adapters.Storages;
public class StorageAdapter
{
    BlobServiceClient _storageClient = new BlobServiceClient(Environment.GetEnvironmentVariable(Envars.Storage_ConnectionString));

    public async Task<bool> Exists(string containerName, string fileName)
    {
        var containerClient = _storageClient.GetBlobContainerClient(containerName);
        var blob = containerClient.GetBlobClient(fileName);
        return await blob.ExistsAsync();
    }

    public async Task<Stream?> Download(string containerName, string fileName)
    {
        var containerClient = _storageClient.GetBlobContainerClient(containerName);
        var blob = containerClient.GetBlobClient(fileName);
        if (!await blob.ExistsAsync())
        {
            return null;
        }
        BlobDownloadInfo downloadInfo = await blob.DownloadAsync();

        return downloadInfo.Content;
    }

    public async Task Upload(string containerName, string fileName, string content)
    {
        var blobContainer = _storageClient.GetBlobContainerClient(containerName);

        var blob = blobContainer.GetBlobClient(fileName);

        using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(content)))
        {
            await blob.UploadAsync(ms);
        }
    }
}
