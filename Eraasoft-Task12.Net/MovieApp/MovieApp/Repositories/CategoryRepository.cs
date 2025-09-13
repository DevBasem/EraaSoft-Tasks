using Microsoft.EntityFrameworkCore;
using MovieApp.DataAccess;
using MovieApp.Models;
using MovieApp.Repositories.IRepositories;

namespace MovieApp.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly MoviesDbContext _db;

        public CategoryRepository(MoviesDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Category>> GetCategoriesWithMoviesCountAsync()
        {
            return await _db.Categories
                .Include(c => c.Movies)
                .ToListAsync();
        }
    }
}