using MovieApp.Models;
using MovieApp.ViewModels;
using MovieApp.ViewModels.Admin;

namespace MovieApp.Services.Interfaces
{
    public interface IActorService
    {
        Task<List<ViewModels.Admin.ActorVM>> GetAllActorsAsync();
        Task<Actor> GetActorByIdAsync(int id);
        Task<Actor> GetActorWithMoviesAsync(int id);
        Task<ViewModels.Admin.ActorVM> GetActorDetailsAsync(int id);
        Task<ActorsIndexVM> GetActorsForPublicIndexAsync();
        Task<ActorDetailsVM> GetActorForPublicDetailsAsync(int id);
        Task<Actor> CreateActorAsync(ActorCreateEditVM viewModel);
        Task<Actor> UpdateActorAsync(int id, ActorCreateEditVM viewModel);
        Task DeleteActorAsync(int id);
        Task<bool> ActorExistsAsync(int id);
    }
}