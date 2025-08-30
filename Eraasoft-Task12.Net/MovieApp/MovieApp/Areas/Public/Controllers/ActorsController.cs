using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieApp.DataAccess;
using MovieApp.ViewModels;

namespace MovieApp.Areas.Public.Controllers
{
    [Area("Public")]
    public class ActorsController : Controller
    {
        private readonly MoviesDbContext _context;

        public ActorsController(MoviesDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var actors = await _context.Actors
                .Include(a => a.Movies)
                    .ThenInclude(ma => ma.Movie)
                .ToListAsync();

            var viewModel = new ActorsIndexVM
            {
                Actors = actors.Select(a => new ActorVM
                {
                    Id = a.Id,
                    FullName = a.FullName,
                    Bio = a.Bio,
                    ProfileImagePath = !string.IsNullOrEmpty(a.ProfileImagePath) ? a.ProfileImagePath : "/images/default-actor.svg",
                    MovieCount = a.Movies.Count(),
                    RecentMovies = a.Movies.Take(3).Select(ma => ma.Movie.Title).ToList()
                }).ToList(),
                TotalActors = actors.Count,
                TotalMovieAppearances = actors.Sum(a => a.Movies.Count())
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var actor = await _context.Actors
                .Include(a => a.Movies)
                    .ThenInclude(ma => ma.Movie)
                        .ThenInclude(m => m.Category)
                .Include(a => a.Movies)
                    .ThenInclude(ma => ma.Movie)
                        .ThenInclude(m => m.Cinema)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (actor == null)
            {
                return NotFound();
            }

            var viewModel = new ActorDetailsVM
            {
                Id = actor.Id,
                FullName = actor.FullName,
                Bio = actor.Bio,
                ProfileImagePath = !string.IsNullOrEmpty(actor.ProfileImagePath) ? actor.ProfileImagePath : "/images/default-actor.svg",
                Movies = actor.Movies.Select(ma => new ActorMovieVM
                {
                    Id = ma.Movie.Id,
                    Title = ma.Movie.Title,
                    ReleaseYear = ma.Movie.ReleaseYear,
                    Status = ma.Movie.Status,
                    PosterUrl = ma.Movie.PosterUrl,
                    Price = ma.Movie.Price,
                    DurationMinutes = ma.Movie.DurationMinutes,
                    CategoryName = ma.Movie.Category?.Name,
                    CinemaName = ma.Movie.Cinema?.Name
                }).OrderByDescending(m => m.ReleaseYear).ToList(),
                TotalMovies = actor.Movies.Count(),
                NowShowingMovies = actor.Movies.Count(ma => ma.Movie.Status == Models.MovieStatus.NowShowing),
                ComingSoonMovies = actor.Movies.Count(ma => ma.Movie.Status == Models.MovieStatus.ComingSoon)
            };

            return View(viewModel);
        }
    }
}