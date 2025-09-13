using MovieApp.Models;

namespace MovieApp.Repositories.IRepositories
{
    public interface IMovieRepository : IRepository<Movie> 
    {
        Task<IEnumerable<Movie>> GetMoviesWithCategoryAndCinema();
        Task<Movie?> GetMovieWithDetailsAsync(int id);
        Task<IEnumerable<Movie>> GetMoviesByStatusAsync(MovieStatus status);
    }
}