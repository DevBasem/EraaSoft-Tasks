using Microsoft.AspNetCore.Http;

namespace MovieApp.Services.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(IFormFile file, string folder = "uploads");
        Task DeleteFileAsync(string filePath);
        string GetFileUrl(string fileName);
    }
}