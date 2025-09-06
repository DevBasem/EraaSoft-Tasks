using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieApp.DataAccess;
using MovieApp.Models;
using MovieApp.Utilities;
using MovieApp.ViewModels.Admin;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApp.Areas.Admin.Controllers
{
    [Area("Admin")]
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
                .Select(c => new CategoryVM
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    MovieCount = c.Movies.Count()
                })
                .OrderBy(c => c.Name)
                .ToListAsync();

            return View(categories);
        }

        public async Task<IActionResult> Details(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Movies)
                    .ThenInclude(m => m.Cinema)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                ToastNotification.Error(TempData, "Category not found");
                return NotFound();
            }

            var viewModel = new CategoryVM
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                MovieCount = category.Movies.Count(),
                Movies = category.Movies.Select(m => new MovieItemVM
                {
                    Id = m.Id,
                    Title = m.Title,
                    ReleaseYear = m.ReleaseYear,
                    Status = m.Status
                }).ToList()
            };

            return View(viewModel);
        }

        public IActionResult Create()
        {
            return View(new CategoryCreateEditVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryCreateEditVM viewModel)
        {
            if (ModelState.IsValid)
            {
                var category = new Category
                {
                    Name = viewModel.Name,
                    Description = viewModel.Description
                };

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
                
                ToastNotification.Success(TempData, $"Category '{category.Name}' was created successfully");
                return RedirectToAction(nameof(Index));
            }
            
            ToastNotification.Error(TempData, "Please correct the errors and try again");
            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                ToastNotification.Error(TempData, "Category not found");
                return NotFound();
            }

            var viewModel = new CategoryCreateEditVM
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryCreateEditVM viewModel)
        {
            if (id != viewModel.Id)
            {
                ToastNotification.Error(TempData, "Invalid category ID");
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var category = await _context.Categories.FindAsync(id);

                    if (category == null)
                    {
                        ToastNotification.Error(TempData, "Category not found");
                        return NotFound();
                    }

                    category.Name = viewModel.Name;
                    category.Description = viewModel.Description;

                    _context.Update(category);
                    await _context.SaveChangesAsync();
                    
                    ToastNotification.Success(TempData, $"Category '{category.Name}' was updated successfully");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(viewModel.Id))
                    {
                        ToastNotification.Error(TempData, "Category not found");
                        return NotFound();
                    }
                    else
                    {
                        ToastNotification.Error(TempData, "An error occurred while updating the category");
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            
            ToastNotification.Error(TempData, "Please correct the errors and try again");
            return View(viewModel);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Movies)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                ToastNotification.Error(TempData, "Category not found");
                return NotFound();
            }

            var viewModel = new CategoryVM
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                MovieCount = category.Movies.Count()
            };

            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            
            if (category != null)
            {
                var categoryName = category.Name;
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                ToastNotification.Success(TempData, $"Category '{categoryName}' was deleted successfully");
            }
            else
            {
                ToastNotification.Error(TempData, "Category not found");
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}