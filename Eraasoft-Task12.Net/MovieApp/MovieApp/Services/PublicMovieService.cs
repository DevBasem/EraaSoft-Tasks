using MovieApp.Models;
using MovieApp.Repositories.IRepositories;
using MovieApp.Services.Interfaces;
using MovieApp.ViewModels;
using System.Linq.Expressions;

namespace MovieApp.Services
{
    public class PublicMovieService : IPublicMovieService
    {
        private readonly IUnitOfWork _unitOfWork;
        private const int DefaultPageSize = 9;

        public PublicMovieService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MoviesIndexVM> GetMoviesIndexAsync(int? page, int? categoryId, int? status, string? sort)
        {
            int pageSize = DefaultPageSize;
            int pageNumber = page ?? 1;

            Expression<Func<Movie, object>>[] includes = { m => m.Category, m => m.Cinema };
            
            var moviesQuery = await _unitOfWork.Movies.GetAllAsync(
                includes: includes,
                tracking: false);

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                moviesQuery = moviesQuery.Where(m => m.CategoryId == categoryId.Value).ToList();
            }

            if (status.HasValue && status.Value > 0)
            {
                moviesQuery = moviesQuery.Where(m => (int)m.Status == status.Value).ToList();
            }

            moviesQuery = sort switch
            {
                "year" => moviesQuery.OrderByDescending(m => m.ReleaseYear).ToList(),
                "price" => moviesQuery.OrderBy(m => m.Price).ToList(),
                "duration" => moviesQuery.OrderByDescending(m => m.DurationMinutes).ToList(),
                _ => moviesQuery.OrderBy(m => m.Title).ToList()
            };

            var totalMovies = moviesQuery.Count;

            var movies = moviesQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var categories = await _unitOfWork.Categories.GetAllAsync();

            var allMovies = await _unitOfWork.Movies.GetAllAsync();

            return new MoviesIndexVM
            {
                Movies = movies.Select(m => new MovieVM
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
                Categories = categories.Select(c => new CategoryVM
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    MovieCount = c.Movies?.Count() ?? 0
                }).ToList(),
                TotalMovies = allMovies.Count,
                NowShowingCount = allMovies.Count(m => m.Status == MovieStatus.NowShowing),
                ComingSoonCount = allMovies.Count(m => m.Status == MovieStatus.ComingSoon),
                Pager = new Pager(totalMovies, pageNumber, pageSize),
                CurrentCategoryId = categoryId,
                CurrentStatus = status,
                CurrentSort = sort
            };
        }

        public async Task<MovieDetailsVM> GetMovieDetailsAsync(int id)
        {
            var movie = await _unitOfWork.Movies.GetMovieWithDetailsAsync(id);

            if (movie == null)
            {
                return null;
            }

            return new MovieDetailsVM
            {
                Id = movie.Id,
                Title = movie.Title,
                Description = movie.Description,
                ReleaseYear = movie.ReleaseYear,
                DurationMinutes = movie.DurationMinutes,
                Status = movie.Status,
                PosterUrl = movie.PosterUrl,
                Price = movie.Price,
                TotalTickets = movie.TotalTickets,
                ReservedTickets = movie.ReservedTickets,
                AvailableTickets = movie.AvailableTickets,
                CategoryId = movie.CategoryId,
                CategoryName = movie.Category?.Name,
                CategoryDescription = movie.Category?.Description,
                CinemaId = movie.CinemaId,
                CinemaName = movie.Cinema?.Name,
                CinemaLocation = movie.Cinema?.Location,
                CinemaAddress = movie.Cinema?.Address,
                CinemaCity = movie.Cinema?.City,
                CinemaPhone = movie.Cinema?.PhoneNumber,
                CinemaEmail = movie.Cinema?.Email,
                Actors = movie.Actors.Select(ma => new ActorVM
                {
                    Id = ma.Actor.Id,
                    FullName = ma.Actor.FullName,
                    Bio = ma.Actor.Bio,
                    ProfileImagePath = ma.Actor.ProfileImagePath
                }).ToList(),
                Images = movie.Images.Select(mi => new MovieImageVM
                {
                    Id = mi.Id,
                    Path = mi.Path,
                    Order = mi.Order
                }).ToList()
            };
        }
    }
}