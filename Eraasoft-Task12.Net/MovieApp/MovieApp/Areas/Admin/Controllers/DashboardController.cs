using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieApp.DataAccess;
using MovieApp.ViewModels.Admin;
using System.Threading.Tasks;

namespace MovieApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly MoviesDbContext _context;

        public DashboardController(MoviesDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new DashboardVM
            {
                TotalMovies = await _context.Movies.CountAsync(),
                TotalActors = await _context.Actors.CountAsync(),
                TotalCategories = await _context.Categories.CountAsync(),
                TotalCinemas = await _context.Cinemas.CountAsync(),
                NowShowingMovies = await _context.Movies.CountAsync(m => m.Status == Models.MovieStatus.NowShowing),
                ComingSoonMovies = await _context.Movies.CountAsync(m => m.Status == Models.MovieStatus.ComingSoon),
                RecentMovies = await _context.Movies
                    .Include(m => m.Category)
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
                    .ToListAsync()
            };

            return View(viewModel);
        }
    }
}