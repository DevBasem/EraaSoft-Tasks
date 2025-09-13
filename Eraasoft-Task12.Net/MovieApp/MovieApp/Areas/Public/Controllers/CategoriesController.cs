using Microsoft.AspNetCore.Mvc;
using MovieApp.Services.Interfaces;
using System.Threading.Tasks;

namespace MovieApp.Areas.Public.Controllers
{
    [Area("Public")]
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = await _categoryService.GetCategoriesForPublicIndexAsync();
            return View(viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var viewModel = await _categoryService.GetCategoryForPublicDetailsAsync(id);

            if (viewModel == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }
    }
}