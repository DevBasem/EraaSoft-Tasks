using Microsoft.EntityFrameworkCore;
using MovieApp.DataAccess;
using MovieApp.Models;
using MovieApp.Repositories.IRepositories;

namespace MovieApp.Repositories
{
    public class ActorRepository : Repository<Actor>, IActorRepository
    {
        private readonly MoviesDbContext _db;

        public ActorRepository(MoviesDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<Actor?> GetActorWithMoviesAsync(int id)
        {
            return await _db.Actors
                .Include(a => a.Movies)
                    .ThenInclude(ma => ma.Movie)
                        .ThenInclude(m => m.Category)
                .Include(a => a.Movies)
                    .ThenInclude(ma => ma.Movie)
                        .ThenInclude(m => m.Cinema)
                .FirstOrDefaultAsync(a => a.Id == id);
        }
    }
}