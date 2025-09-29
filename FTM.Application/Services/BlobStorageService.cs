using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FTM.Application.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FTM.Application.Services
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly IConfiguration _configuration;

        public BlobStorageService(IConfiguration configuration)
        {
            _configuration = configuration;
            var connectionString = _configuration.GetConnectionString("BlobStorage") 
                ?? Environment.GetEnvironmentVariable("BLOB_STORAGE_CONNECTION_STRING")
                ?? "UseDevelopmentStorage=true"; // For Azurite local development
            
            _blobServiceClient = new BlobServiceClient(connectionString);
        }

        public async Task<string> UploadFileAsync(IFormFile file, string containerName, string fileName = null)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File không hợp lệ", nameof(file));

            // Validate file type (only images)
            var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
            if (!allowedTypes.Contains(file.ContentType.ToLower()))
                throw new ArgumentException("Chỉ chấp nhận file ảnh (JPEG, PNG, GIF, WebP)");

            // Validate file size (max 5MB)
            if (file.Length > 5 * 1024 * 1024)
                throw new ArgumentException("Kích thước file không được vượt quá 5MB");

            fileName ??= GenerateUniqueFileName(file.FileName);

            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            
            // Create container if it doesn't exist
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var blobClient = containerClient.GetBlobClient(fileName);

            using var stream = file.OpenReadStream();
            
            var blobHttpHeaders = new BlobHttpHeaders
            {
                ContentType = file.ContentType
            };

            await blobClient.UploadAsync(stream, new BlobUploadOptions
            {
                HttpHeaders = blobHttpHeaders
            });

            return blobClient.Uri.ToString();
        }

        public async Task<bool> DeleteFileAsync(string containerName, string fileName)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                var blobClient = containerClient.GetBlobClient(fileName);

                var response = await blobClient.DeleteIfExistsAsync();
                return response.Value;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> GetFileUrlAsync(string containerName, string fileName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            var exists = await blobClient.ExistsAsync();
            return exists.Value ? blobClient.Uri.ToString() : null;
        }

        public string GenerateUniqueFileName(string originalFileName)
        {
            var extension = Path.GetExtension(originalFileName);
            var fileName = Path.GetFileNameWithoutExtension(originalFileName);
            
            // Create unique filename with timestamp
            var uniqueFileName = $"{fileName}_{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid():N}{extension}";
            
            return uniqueFileName;
        }
    }
}