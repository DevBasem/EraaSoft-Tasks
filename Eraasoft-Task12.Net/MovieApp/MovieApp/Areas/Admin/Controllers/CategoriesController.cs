using Microsoft.AspNetCore.Mvc;
using MovieApp.Areas.Admin;
using MovieApp.Services.Interfaces;
using MovieApp.Utilities;
using MovieApp.ViewModels.Admin;
using System;
using System.Threading.Tasks;

namespace MovieApp.Areas.Admin.Controllers
{
    [AdminAreaAttribute]
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            var viewModels = await _categoryService.GetAllCategoriesAsync();
            return View(viewModels);
        }

        public async Task<IActionResult> Details(int id)
        {
            var viewModel = await _categoryService.GetCategoryDetailsAsync(id);

            if (viewModel == null)
            {
                ToastNotification.Error(TempData, "Category not found");
                return NotFound();
            }

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
                var category = await _categoryService.CreateCategoryAsync(viewModel);
                ToastNotification.Success(TempData, $"Category '{category.Name}' was created successfully");
                return RedirectToAction(nameof(Index));
            }
            
            ToastNotification.Error(TempData, "Please correct the errors and try again");
            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);

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
                    var category = await _categoryService.UpdateCategoryAsync(id, viewModel);

                    if (category == null)
                    {
                        ToastNotification.Error(TempData, "Category not found");
                        return NotFound();
                    }

                    ToastNotification.Success(TempData, $"Category '{category.Name}' was updated successfully");
                }
                catch (Exception)
                {
                    if (!await _categoryService.CategoryExistsAsync(viewModel.Id))
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
            var viewModel = await _categoryService.GetCategoryDetailsAsync(id);

            if (viewModel == null)
            {
                ToastNotification.Error(TempData, "Category not found");
                return NotFound();
            }

            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            
            if (category != null)
            {
                var categoryName = category.Name;
                await _categoryService.DeleteCategoryAsync(id);
                ToastNotification.Success(TempData, $"Category '{categoryName}' was deleted successfully");
            }
            else
            {
                ToastNotification.Error(TempData, "Category not found");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}