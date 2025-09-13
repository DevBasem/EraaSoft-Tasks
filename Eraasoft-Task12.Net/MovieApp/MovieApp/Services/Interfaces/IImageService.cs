using MovieApp.Models;

namespace MovieApp.Services.Interfaces
{
    public interface IImageService
    {
        Task<MovieImage> GetImageByIdAsync(int imageId);
        Task DeleteImageAsync(int imageId);
        Task UpdateImageOrderAsync(int imageId, int newOrder);
    }
}