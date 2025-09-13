using MovieApp.DataAccess;
using MovieApp.Repositories.IRepositories;

namespace MovieApp.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MoviesDbContext _db;
        public IMovieRepository Movies { get; private set; }
        public ICategoryRepository Categories { get; private set; }
        public ICinemaRepository Cinemas { get; private set; }
        public IActorRepository Actors { get; private set; }

        public UnitOfWork(MoviesDbContext db)
        {
            _db = db;
            Movies = new MovieRepository(_db);
            Categories = new CategoryRepository(_db);
            Cinemas = new CinemaRepository(_db);
            Actors = new ActorRepository(_db);
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}