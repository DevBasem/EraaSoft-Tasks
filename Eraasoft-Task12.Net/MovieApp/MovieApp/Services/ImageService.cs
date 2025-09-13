using Microsoft.EntityFrameworkCore;
using MovieApp.Models;
using MovieApp.Repositories.IRepositories;
using MovieApp.Services.Interfaces;

namespace MovieApp.Services
{
    public class ImageService : IImageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorageService _fileStorageService;

        public ImageService(IUnitOfWork unitOfWork, IFileStorageService fileStorageService)
        {
            _unitOfWork = unitOfWork;
            _fileStorageService = fileStorageService;
        }

        public async Task<MovieImage> GetImageByIdAsync(int imageId)
        {
            var dbContext = _unitOfWork.Movies.GetDbContext();
            return await dbContext.MovieImages.FindAsync(imageId);
        }

        public async Task DeleteImageAsync(int imageId)
        {
            var dbContext = _unitOfWork.Movies.GetDbContext();
            var image = await dbContext.MovieImages.FindAsync(imageId);
            if (image == null)
                return;

            // Delete the actual file
            await _fileStorageService.DeleteFileAsync(image.Path);
            
            // Remove from database
            dbContext.MovieImages.Remove(image);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateImageOrderAsync(int imageId, int newOrder)
        {
            var dbContext = _unitOfWork.Movies.GetDbContext();
            var image = await dbContext.MovieImages.FindAsync(imageId);
            if (image == null)
                return;

            var movieImagesQuery = dbContext.MovieImages
                .Where(mi => mi.MovieId == image.MovieId)
                .OrderBy(mi => mi.Order);
            
            var movieImages = await movieImagesQuery.ToListAsync();
            
            if (newOrder < 1)
            {
                newOrder = 1;
            }
            else if (newOrder > movieImages.Count)
            {
                newOrder = movieImages.Count;
            }
            
            int oldOrder = image.Order;
            
            if (newOrder < oldOrder)
            {
                foreach (var img in movieImages.Where(mi => mi.Order >= newOrder && mi.Order < oldOrder))
                {
                    img.Order++;
                }
            }
            else if (newOrder > oldOrder)
            {
                foreach (var img in movieImages.Where(mi => mi.Order <= newOrder && mi.Order > oldOrder))
                {
                    img.Order--;
                }
            }
            
            image.Order = newOrder;
            
            await _unitOfWork.SaveChangesAsync();
        }
    }
}