using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieApp.DataAccess;
using MovieApp.ViewModels;

namespace MovieApp.Areas.Public.Controllers
{
    [Area("Public")]
    public class CategoriesController : Controller
    {
        private readonly MoviesDbContext _context;

        public CategoriesController(MoviesDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories
                .Include(c => c.Movies)
                .ToListAsync();

            var mostPopular = categories.OrderByDescending(c => c.Movies.Count()).FirstOrDefault();

            var viewModel = new CategoriesIndexVM
            {
                Categories = categories.Select(c => new CategoryVM
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    MovieCount = c.Movies.Count(),
                    NowShowingCount = c.Movies.Count(m => m.Status == Models.MovieStatus.NowShowing),
                    ComingSoonCount = c.Movies.Count(m => m.Status == Models.MovieStatus.ComingSoon)
                }).ToList(),
                TotalCategories = categories.Count,
                TotalMovies = categories.Sum(c => c.Movies.Count()),
                MostPopularCategory = mostPopular?.Name ?? "N/A"
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Movies)
                    .ThenInclude(m => m.Cinema)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            var viewModel = new CategoryDetailsVM
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
                    CategoryName = m.Category?.Name,
                    CinemaName = m.Cinema?.Name
                }).ToList(),
                TotalMovies = category.Movies.Count,
                NowShowingMovies = category.Movies.Count(m => m.Status == Models.MovieStatus.NowShowing),
                ComingSoonMovies = category.Movies.Count(m => m.Status == Models.MovieStatus.ComingSoon)
            };

            return View(viewModel);
        }
    }
}