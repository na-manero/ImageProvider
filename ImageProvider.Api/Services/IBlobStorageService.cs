namespace ImageProvider.Api.Services;

public interface IBlobStorageService
{
    Task<string> UploadImageAsync(IFormFile file, string containerName);
    Task<bool> DeleteImageAsync(string imageName, string containerName);
}
