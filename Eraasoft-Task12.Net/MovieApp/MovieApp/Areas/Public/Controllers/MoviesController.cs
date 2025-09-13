using Microsoft.AspNetCore.Mvc;
using MovieApp.Services.Interfaces;
using System.Threading.Tasks;

namespace MovieApp.Areas.Public.Controllers
{
    [Area("Public")]
    public class MoviesController : Controller
    {
        private readonly IPublicMovieService _movieService;

        public MoviesController(IPublicMovieService movieService)
        {
            _movieService = movieService;
        }

        public async Task<IActionResult> Index(int? page, int? categoryId, int? status, string? sort)
        {
            var viewModel = await _movieService.GetMoviesIndexAsync(page, categoryId, status, sort);
            return View(viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var viewModel = await _movieService.GetMovieDetailsAsync(id);

            if (viewModel == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }
    }
}