using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieApp.DataAccess;
using MovieApp.Models;
using MovieApp.ViewModels;

namespace MovieApp.Areas.Public.Controllers
{
    [Area("Public")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MoviesDbContext _context;

        public HomeController(ILogger<HomeController> logger, MoviesDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var featuredMovies = await _context.Movies
                .Include(m => m.Category)
                .Include(m => m.Cinema)
                .Where(m => m.Status == MovieStatus.NowShowing)
                .Take(6)
                .ToListAsync();

            var totalMovies = await _context.Movies.CountAsync();
            var totalActors = await _context.Actors.CountAsync();
            var totalCategories = await _context.Categories.CountAsync();
            var totalCinemas = await _context.Cinemas.CountAsync();

            var viewModel = new HomeIndexVM
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

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}