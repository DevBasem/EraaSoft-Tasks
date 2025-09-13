using MovieApp.Models;

namespace MovieApp.Repositories.IRepositories
{
    public interface IActorRepository : IRepository<Actor>
    {
        Task<Actor?> GetActorWithMoviesAsync(int id);
    }
}