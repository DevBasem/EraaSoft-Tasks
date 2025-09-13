using MovieApp.Models;
using MovieApp.Repositories.IRepositories;
using MovieApp.Services.Interfaces;
using MovieApp.ViewModels;
using System.Linq.Expressions;

namespace MovieApp.Services
{
    public class HomeService : IHomeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public HomeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<HomeIndexVM> GetHomePageDataAsync()
        {
            Expression<Func<Movie, object>>[] includes = { m => m.Category, m => m.Cinema };
            
            var movies = await _unitOfWork.Movies.GetAllAsync(
                filter: m => m.Status == MovieStatus.NowShowing,
                includes: includes
            );
            
            var featuredMovies = movies.Take(6).ToList();
            
            var totalMovies = await _unitOfWork.Movies.CountAsync();
            var totalActors = await _unitOfWork.Actors.CountAsync();
            var totalCategories = await _unitOfWork.Categories.CountAsync();
            var totalCinemas = await _unitOfWork.Cinemas.CountAsync();

            return new HomeIndexVM
            {
                FeaturedMovies = featuredMovies.Select(m => new MovieVM
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
                    CategoryName = m.Category?.Name,
                    CinemaName = m.Cinema?.Name
                }).ToList(),
                TotalMovies = totalMovies,
                TotalActors = totalActors,
                TotalCategories = totalCategories,
                TotalCinemas = totalCinemas
            };
        }
    }
}