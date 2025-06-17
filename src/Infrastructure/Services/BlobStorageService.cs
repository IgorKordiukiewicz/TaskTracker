using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services;

public interface IBlobStorageService
{
    Task UploadFile(IFormFile file, string path);
}

public class BlobStorageService : IBlobStorageService
{
    public async Task UploadFile(IFormFile file, string path)
    {
        // TODO
    }
}
