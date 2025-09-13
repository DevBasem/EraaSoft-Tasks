using MovieApp.Models;
using MovieApp.ViewModels.Admin;

namespace MovieApp.Services.Interfaces
{
    public interface ICinemaService
    {
        Task<List<CinemaVM>> GetAllCinemasAsync();
        Task<Cinema> GetCinemaByIdAsync(int id);
        Task<CinemaVM> GetCinemaDetailsAsync(int id);
        Task<Cinema> CreateCinemaAsync(CinemaCreateEditVM viewModel);
        Task<Cinema> UpdateCinemaAsync(int id, CinemaCreateEditVM viewModel);
        Task DeleteCinemaAsync(int id);
        Task<bool> CinemaExistsAsync(int id);
    }
}