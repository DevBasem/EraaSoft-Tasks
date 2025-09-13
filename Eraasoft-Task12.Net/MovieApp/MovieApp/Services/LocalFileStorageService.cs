using Microsoft.AspNetCore.Http;
using MovieApp.Services.Interfaces;

namespace MovieApp.Services
{
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly string _contentRoot;

        public LocalFileStorageService(IWebHostEnvironment environment)
        {
            _environment = environment;
            _contentRoot = _environment.WebRootPath;
        }

        public async Task<string> SaveFileAsync(IFormFile file, string folder = "uploads")
        {
            if (file == null || file.Length == 0)
            {
                return null;
            }

            var uploadsDir = Path.Combine(_contentRoot, folder);
            
            if (!Directory.Exists(uploadsDir))
            {
                Directory.CreateDirectory(uploadsDir);
            }
            
            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsDir, uniqueFileName);
            
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            
            return $"/{folder}/{uniqueFileName}";
        }

        public Task DeleteFileAsync(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return Task.CompletedTask;
            }

            // Remove starting slash if present
            if (filePath.StartsWith("/"))
            {
                filePath = filePath.Substring(1);
            }

            string fullPath = Path.Combine(_contentRoot, filePath);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            return Task.CompletedTask;
        }

        public string GetFileUrl(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return null;
            }

            // Ensure we have a proper URL format starting with /
            if (!fileName.StartsWith("/"))
            {
                fileName = $"/{fileName}";
            }

            return fileName;
        }
    }
}