using MovieApp.Models;

namespace MovieApp.Repositories.IRepositories
{
    public interface ICinemaRepository : IRepository<Cinema>
    {
        Task<IEnumerable<Cinema>> GetCinemasWithMoviesCountAsync();
    }
}