using MovieApp.Models;
using MovieApp.Repositories.IRepositories;
using MovieApp.Services.Interfaces;
using MovieApp.ViewModels.Admin;
using System.Linq.Expressions;

namespace MovieApp.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DashboardVM> GetDashboardDataAsync()
        {
            var totalMovies = await _unitOfWork.Movies.CountAsync();
            var totalActors = await _unitOfWork.Actors.CountAsync();
            var totalCategories = await _unitOfWork.Categories.CountAsync();
            var totalCinemas = await _unitOfWork.Cinemas.CountAsync();
            var nowShowingMovies = await _unitOfWork.Movies.CountAsync(m => m.Status == MovieStatus.NowShowing);
            var comingSoonMovies = await _unitOfWork.Movies.CountAsync(m => m.Status == MovieStatus.ComingSoon);
            
            Expression<Func<Movie, object>>[] includes = { m => m.Category };
            var allMovies = await _unitOfWork.Movies.GetAllAsync(includes: includes);
            var recentMovies = allMovies
                .OrderByDescending(m => m.Id)
                .Take(5)
                .Select(m => new MovieItemVM
                {
                    Id = m.Id,
                    Title = m.Title,
                    CategoryName = m.Category != null ? m.Category.Name : "Uncategorized",
                    Status = m.Status,
                    ReleaseYear = m.ReleaseYear
                })
                .ToList();

            return new DashboardVM
            {
                TotalMovies = totalMovies,
                TotalActors = totalActors,
                TotalCategories = totalCategories,
                TotalCinemas = totalCinemas,
                NowShowingMovies = nowShowingMovies,
                ComingSoonMovies = comingSoonMovies,
                RecentMovies = recentMovies
            };
        }
    }
}