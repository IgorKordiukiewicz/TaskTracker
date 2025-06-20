using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

public interface IBlobStorageService
{
    Task UploadFile(IFormFile file, string path);
    Task<string> GetDownloadUrl(string path);
}

public class BlobStorageService(BlobServiceClient blobServiceClient, IOptions<InfrastructureSettings> infrastructureSettings)
    : IBlobStorageService
{
    private const int ExpirationHours = 1;

    public async Task UploadFile(IFormFile file, string path)
    {
        var blobContainerClient = blobServiceClient.GetBlobContainerClient(infrastructureSettings.Value.Blob.Container);
        var blobClient = blobContainerClient.GetBlobClient(path);

        using var stream = file.OpenReadStream();
        await blobClient.UploadAsync(stream, overwrite: true);
    }

    public async Task<string> GetDownloadUrl(string path)
    {
        var blobContainerClient = blobServiceClient.GetBlobContainerClient(infrastructureSettings.Value.Blob.Container);

        var blobClient = blobContainerClient.GetBlobClient(path);
        if (!await blobClient.ExistsAsync() || !blobClient.CanGenerateSasUri)
        {
            return string.Empty;
        }

        var sasUri = blobClient.GenerateSasUri(BlobSasPermissions.Read, DateTime.UtcNow.AddHours(ExpirationHours));
        return sasUri.ToString();
    }
}
