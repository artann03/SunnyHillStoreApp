using Microsoft.AspNetCore.Http;


namespace SunnyHillStore.Core.Services.External
{
    public interface ICloudinaryService
    {
        Task<string> UploadImageAsync(IFormFile file);
        Task<bool> DeleteImageAsync(string publicId);
    }
}