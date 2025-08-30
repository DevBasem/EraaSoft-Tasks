using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieApp.DataAccess;
using MovieApp.ViewModels;

namespace MovieApp.Areas.Public.Controllers
{
    [Area("Public")]
    public class MoviesController : Controller
    {
        private readonly MoviesDbContext _context;

        public MoviesController(MoviesDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? page, int? categoryId, int? status, string? sort)
        {
            const int pageSize = 9; // 9 movies per page for a 3x3 grid
            int pageNumber = page ?? 1;

            var moviesQuery = _context.Movies
                .Include(m => m.Category)
                .Include(m => m.Cinema)
                .AsQueryable();

            // Apply filters
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                moviesQuery = moviesQuery.Where(m => m.CategoryId == categoryId.Value);
            }

            if (status.HasValue && status.Value > 0)
            {
                moviesQuery = moviesQuery.Where(m => (int)m.Status == status.Value);
            }

            // Apply sorting
            moviesQuery = sort switch
            {
                "year" => moviesQuery.OrderByDescending(m => m.ReleaseYear),
                "price" => moviesQuery.OrderBy(m => m.Price),
                "duration" => moviesQuery.OrderByDescending(m => m.DurationMinutes),
                _ => moviesQuery.OrderBy(m => m.Title)
            };

            // Get total count for pagination
            var totalMovies = await moviesQuery.CountAsync();

            // Apply pagination
            var movies = await moviesQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var categories = await _context.Categories.ToListAsync();

            // Get all movies for stats (not filtered)
            var allMovies = await _context.Movies.ToListAsync();

            var viewModel = new MoviesIndexVM
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
                    MovieCount = c.Movies.Count()
                }).ToList(),
                TotalMovies = allMovies.Count,
                NowShowingCount = allMovies.Count(m => m.Status == Models.MovieStatus.NowShowing),
                ComingSoonCount = allMovies.Count(m => m.Status == Models.MovieStatus.ComingSoon),
                Pager = new Pager(totalMovies, pageNumber, pageSize),
                CurrentCategoryId = categoryId,
                CurrentStatus = status,
                CurrentSort = sort
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var movie = await _context.Movies
                .Include(m => m.Category)
                .Include(m => m.Cinema)
                .Include(m => m.Actors)
                    .ThenInclude(ma => ma.Actor)
                .Include(m => m.Images)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            var viewModel = new MovieDetailsVM
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

            return View(viewModel);
        }
    }
}