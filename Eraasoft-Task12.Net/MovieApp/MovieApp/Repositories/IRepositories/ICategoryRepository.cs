using MovieApp.Models;

namespace MovieApp.Repositories.IRepositories
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<IEnumerable<Category>> GetCategoriesWithMoviesCountAsync();
    }
}