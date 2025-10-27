using Microsoft.AspNetCore.Http;

namespace FTM.Application.IServices
{
    public interface IBlobStorageService
    {
        Task<string> UploadFileAsync(IFormFile file, string containerName, string fileName = null);
        Task<bool> DeleteFileAsync(string containerName, string fileName);
        Task<string> GetFileUrlAsync(string containerName, string fileName);
        string GenerateUniqueFileName(string originalFileName);
        Task<string> UploadEventImageAsync(IFormFile file, string containerName);
    }
}