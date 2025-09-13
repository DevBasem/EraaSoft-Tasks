using MovieApp.Models;
using MovieApp.Repositories.IRepositories;
using MovieApp.Services.Interfaces;
using MovieApp.ViewModels;
using MovieApp.ViewModels.Admin;
using System.Linq.Expressions;

namespace MovieApp.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ViewModels.Admin.CategoryVM>> GetAllCategoriesAsync()
        {
            var categories = await _unitOfWork.Categories.GetCategoriesWithMoviesCountAsync();
            
            return categories.Select(c => new ViewModels.Admin.CategoryVM
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                MovieCount = c.Movies?.Count() ?? 0
            })
            .OrderBy(c => c.Name)
            .ToList();
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            return await _unitOfWork.Categories.GetByIdAsync(id);
        }

        public async Task<ViewModels.Admin.CategoryVM> GetCategoryDetailsAsync(int id)
        {
            Expression<Func<Category, object>>[] includes = { c => c.Movies };
            
            var category = await _unitOfWork.Categories.GetFirstOrDefaultAsync(
                filter: c => c.Id == id,
                includes: includes);

            if (category == null)
                return null;

            return new ViewModels.Admin.CategoryVM
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                MovieCount = category.Movies?.Count() ?? 0,
                Movies = category.Movies?.Select(m => new MovieItemVM
                {
                    Id = m.Id,
                    Title = m.Title,
                    ReleaseYear = m.ReleaseYear,
                    Status = m.Status
                }).ToList()
            };
        }

        public async Task<CategoriesIndexVM> GetCategoriesForPublicIndexAsync()
        {
            Expression<Func<Category, object>>[] includes = { c => c.Movies };
            var categories = await _unitOfWork.Categories.GetAllAsync(
                includes: includes
            );

            var mostPopular = categories.OrderByDescending(c => c.Movies.Count()).FirstOrDefault();

            return new CategoriesIndexVM
            {
                Categories = categories.Select(c => new ViewModels.CategoryVM
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    MovieCount = c.Movies.Count(),
                    NowShowingCount = c.Movies.Count(m => m.Status == MovieStatus.NowShowing),
                    ComingSoonCount = c.Movies.Count(m => m.Status == MovieStatus.ComingSoon)
                }).ToList(),
                TotalCategories = categories.Count,
                TotalMovies = categories.Sum(c => c.Movies.Count()),
                MostPopularCategory = mostPopular?.Name ?? "N/A"
            };
        }

        public async Task<CategoryDetailsVM> GetCategoryForPublicDetailsAsync(int id)
        {
            Expression<Func<Category, object>>[] includes = { c => c.Movies };
            var category = await _unitOfWork.Categories.GetFirstOrDefaultAsync(
                c => c.Id == id,
                includes: includes
            );

            if (category == null)
                return null;

            var movieIds = category.Movies.Select(m => m.Id).ToList();
            Expression<Func<Movie, object>>[] movieIncludes = { m => m.Cinema };
            var movies = await _unitOfWork.Movies.GetAllAsync(
                filter: m => movieIds.Contains(m.Id),
                includes: movieIncludes
            );

            // Update movies with cinema information
            foreach (var movie in category.Movies)
            {
                var movieWithCinema = movies.FirstOrDefault(m => m.Id == movie.Id);
                if (movieWithCinema != null)
                {
                    movie.Cinema = movieWithCinema.Cinema;
                }
            }

            return new CategoryDetailsVM
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Movies = category.Movies.Select(m => new MovieVM
                {
                    Id = m.Id,
                    Title = m.Title,
                    Description = m.Description,
                    ReleaseYear = m.ReleaseYear,
                    DurationMinutes = m.DurationMinutes,
                    Status = m.Status,
                    PosterUrl = m.PosterUrl,
                    Price = m.Price,
                    TotalTickets = m.TotalTickets,
                    ReservedTickets = m.ReservedTickets,
                    AvailableTickets = m.AvailableTickets,
                    CategoryName = category.Name,
                    CinemaName = m.Cinema?.Name
                }).ToList(),
                TotalMovies = category.Movies.Count,
                NowShowingMovies = category.Movies.Count(m => m.Status == MovieStatus.NowShowing),
                ComingSoonMovies = category.Movies.Count(m => m.Status == MovieStatus.ComingSoon)
            };
        }

        public async Task<Category> CreateCategoryAsync(CategoryCreateEditVM viewModel)
        {
            var category = new Category
            {
                Name = viewModel.Name,
                Description = viewModel.Description
            };

            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();
            
            return category;
        }

        public async Task<Category> UpdateCategoryAsync(int id, CategoryCreateEditVM viewModel)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);

            if (category == null)
                return null;

            category.Name = viewModel.Name;
            category.Description = viewModel.Description;

            _unitOfWork.Categories.Update(category);
            await _unitOfWork.SaveChangesAsync();
            
            return category;
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            
            if (category != null)
            {
                _unitOfWork.Categories.Remove(category);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<bool> CategoryExistsAsync(int id)
        {
            return await _unitOfWork.Categories.AnyAsync(c => c.Id == id);
        }
    }
}