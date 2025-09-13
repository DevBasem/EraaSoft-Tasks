using Microsoft.EntityFrameworkCore;
using MovieApp.DataAccess;
using MovieApp.Models;
using MovieApp.Repositories.IRepositories;

namespace MovieApp.Repositories
{
    public class MovieRepository : Repository<Movie>, IMovieRepository
    {
        private readonly MoviesDbContext _db;

        public MovieRepository(MoviesDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Movie>> GetMoviesWithCategoryAndCinema()
        {
            return await _db.Movies
                .Include(m => m.Category)
                .Include(m => m.Cinema)
                .ToListAsync();
        }

        public async Task<Movie?> GetMovieWithDetailsAsync(int id)
        {
            return await _db.Movies
                .Include(m => m.Category)
                .Include(m => m.Cinema)
                .Include(m => m.Actors)
                    .ThenInclude(ma => ma.Actor)
                .Include(m => m.Images)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<Movie>> GetMoviesByStatusAsync(MovieStatus status)
        {
            return await _db.Movies
                .Include(m => m.Category)
                .Include(m => m.Cinema)
                .Where(m => m.Status == status)
                .ToListAsync();
        }
    }
}