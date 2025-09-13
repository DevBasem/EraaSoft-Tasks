using Microsoft.EntityFrameworkCore;
using MovieApp.DataAccess;
using MovieApp.Models;
using MovieApp.Repositories.IRepositories;

namespace MovieApp.Repositories
{
    public class CinemaRepository : Repository<Cinema>, ICinemaRepository
    {
        private readonly MoviesDbContext _db;

        public CinemaRepository(MoviesDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Cinema>> GetCinemasWithMoviesCountAsync()
        {
            return await _db.Cinemas
                .Include(c => c.Movies)
                .ToListAsync();
        }
    }
}