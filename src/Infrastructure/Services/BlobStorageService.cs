using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

public interface IBlobStorageService
{
    Task UploadFile(IFormFile file, string path);
}

public class BlobStorageService(BlobServiceClient blobServiceClient, IOptions<InfrastructureSettings> infrastructureSettings)
    : IBlobStorageService
{
    public async Task UploadFile(IFormFile file, string path)
    {
        var blobContainerClient = blobServiceClient.GetBlobContainerClient(infrastructureSettings.Value.Blob.Container);
        var blobClient = blobContainerClient.GetBlobClient(path);

        using var stream = file.OpenReadStream();
        await blobClient.UploadAsync(stream, overwrite: true);
    }
}
