using Microsoft.AspNetCore.Mvc;
using MovieApp.Services.Interfaces;
using System.Threading.Tasks;

namespace MovieApp.Areas.Public.Controllers
{
    [Area("Public")]
    public class ActorsController : Controller
    {
        private readonly IActorService _actorService;

        public ActorsController(IActorService actorService)
        {
            _actorService = actorService;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = await _actorService.GetActorsForPublicIndexAsync();
            return View(viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var viewModel = await _actorService.GetActorForPublicDetailsAsync(id);

            if (viewModel == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }
    }
}