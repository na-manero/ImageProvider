using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Diagnostics;

namespace ImageProvider.Api.Services;

public class BlobStorageService(BlobServiceClient blobServiceClient, IConfiguration configuration) : IBlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient = blobServiceClient;
    private readonly IConfiguration _configuration = configuration;

    public async Task<string> UploadImageAsync(IFormFile file, string containerName)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();
            var blobClient = containerClient.GetBlobClient(file.FileName);

            var blobHttpHeaders = new BlobHttpHeaders
            {
                ContentType = file.ContentType
            };

            using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, new BlobUploadOptions { HttpHeaders = blobHttpHeaders });

            return blobClient.Uri.ToString();
        }
        catch (Exception ex)
        {
            Debug.WriteLine("StorageService : " + ex.Message);
            return string.Empty;
        }
    }

    public async Task<bool> DeleteImageAsync(string imageName, string containerName)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(imageName);

            return await blobClient.DeleteIfExistsAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine("StorageService : " + ex.Message);
            return false;
        }
    }
}
