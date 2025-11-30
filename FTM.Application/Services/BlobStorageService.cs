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

        public BlobStorageService()
        {
            var connectionString = Environment.GetEnvironmentVariable("BLOBSTORAGE")
                ?? Environment.GetEnvironmentVariable("BLOB_STORAGE_CONNECTION_STRING")
                ?? "UseDevelopmentStorage=true"; // For Azurite local development
            
            _blobServiceClient = new BlobServiceClient(connectionString);
        }

        public async Task<string> UploadFileAsync(IFormFile file, string containerName, string? fileName = null)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File không hợp lệ", nameof(file));

            // Validate file type - Support Images, Videos, Icons, Documents
            var allowedTypes = new[] 
            { 
                // Images
                "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp", "image/bmp", "image/svg+xml",
                // Videos
                "video/mp4", "video/mpeg", "video/quicktime", "video/x-msvideo", "video/x-ms-wmv", "video/webm",
                // Documents
                "application/pdf", 
                "application/msword", 
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document", // .docx
                "application/vnd.ms-excel", 
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", // .xlsx
                "application/vnd.ms-powerpoint",
                "application/vnd.openxmlformats-officedocument.presentationml.presentation", // .pptx
                "text/plain",
                // Icons
                "image/x-icon", "image/vnd.microsoft.icon"
            };
            
            if (!allowedTypes.Contains(file.ContentType.ToLower()))
                throw new ArgumentException($"Loại file không được hỗ trợ: {file.ContentType}. Chỉ chấp nhận ảnh, video, document và icon.");

            // Validate file size based on type
            long maxFileSize;
            if (file.ContentType.StartsWith("video/"))
            {
                maxFileSize = 100 * 1024 * 1024; // 100MB for videos
            }
            else if (file.ContentType.StartsWith("application/"))
            {
                maxFileSize = 20 * 1024 * 1024; // 20MB for documents
            }
            else
            {
                maxFileSize = 10 * 1024 * 1024; // 10MB for images and icons
            }

            if (file.Length > maxFileSize)
                throw new ArgumentException($"Kích thước file không được vượt quá {maxFileSize / (1024 * 1024)}MB");

            // Determine subfolder based on file type
            string subFolder = GetSubFolderByContentType(file.ContentType);
            
            fileName ??= GenerateUniqueFileName(file.FileName);
            
            // Combine subfolder with filename: posts/images/filename.jpg
            string blobPath = $"{subFolder}/{fileName}";

            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            
            // Create container if it doesn't exist
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var blobClient = containerClient.GetBlobClient(blobPath);

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

        /// <summary>
        /// Determine subfolder based on content type
        /// </summary>
        private string GetSubFolderByContentType(string contentType)
        {
            if (contentType.StartsWith("image/"))
            {
                // Icons also go to images folder
                return "images";
            }
            else if (contentType.StartsWith("video/"))
            {
                return "videos";
            }
            else if (contentType.StartsWith("application/") || contentType.StartsWith("text/"))
            {
                return "documents";
            }

            // Default fallback
            return "others";
        }

        /// <summary>
        /// Upload file to event folder specifically
        /// </summary>
        public async Task<string> UploadEventImageAsync(IFormFile file, string containerName)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File không hợp lệ", nameof(file));

            // Validate file type - Only images for events
            var allowedTypes = new[]
            {
                "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp", "image/bmp", "image/svg+xml"
            };

            if (!allowedTypes.Contains(file.ContentType.ToLower()))
                throw new ArgumentException($"Loại file không được hỗ trợ: {file.ContentType}. Chỉ chấp nhận ảnh cho sự kiện.");

            // Validate file size - 5MB for event images
            const long maxFileSize = 5 * 1024 * 1024; // 5MB
            if (file.Length > maxFileSize)
                throw new ArgumentException($"Kích thước file không được vượt quá {maxFileSize / (1024 * 1024)}MB");

            var fileName = GenerateUniqueFileName(file.FileName);

            // Use "event" folder for all event images
            string blobPath = $"event/{fileName}";

            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            // Create container if it doesn't exist
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var blobClient = containerClient.GetBlobClient(blobPath);

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
    }
}