using MovieApp.Models;
using MovieApp.ViewModels;
using MovieApp.ViewModels.Admin;

namespace MovieApp.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<List<ViewModels.Admin.CategoryVM>> GetAllCategoriesAsync();
        Task<Category> GetCategoryByIdAsync(int id);
        Task<ViewModels.Admin.CategoryVM> GetCategoryDetailsAsync(int id);
        Task<CategoriesIndexVM> GetCategoriesForPublicIndexAsync();
        Task<CategoryDetailsVM> GetCategoryForPublicDetailsAsync(int id);
        Task<Category> CreateCategoryAsync(CategoryCreateEditVM viewModel);
        Task<Category> UpdateCategoryAsync(int id, CategoryCreateEditVM viewModel);
        Task DeleteCategoryAsync(int id);
        Task<bool> CategoryExistsAsync(int id);
    }
}