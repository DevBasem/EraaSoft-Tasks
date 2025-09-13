using MovieApp.Repositories.IRepositories;

namespace MovieApp.Repositories.IRepositories
{
    public interface IUnitOfWork
    {
        IMovieRepository Movies { get; }
        ICategoryRepository Categories { get; }
        ICinemaRepository Cinemas { get; }
        IActorRepository Actors { get; }
        Task SaveChangesAsync();
    }
}