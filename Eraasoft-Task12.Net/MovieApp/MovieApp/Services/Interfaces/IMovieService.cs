using MovieApp.Models;
using MovieApp.Services;
using MovieApp.ViewModels.Admin;

namespace MovieApp.Services.Interfaces
{
    public interface IMovieService
    {
        Task<MovieItemVMCollection> GetPagedMoviesAsync(
            int page = 1,
            string searchTerm = "",
            string sortField = "Title",
            string sortOrder = "asc",
            string statusFilter = "all",
            int pageSize = 10);
            
        Task<Movie> GetMovieByIdAsync(int id);
        Task<Movie> GetMovieWithDetailsAsync(int id);
        Task<MovieDetailsVM> GetMovieDetailsAsync(int id);
        Task<MovieCreateEditVM> PrepareMovieViewModelForCreateAsync();
        Task<MovieCreateEditVM> PrepareMovieViewModelForEditAsync(int id);
        Task<Movie> CreateMovieAsync(MovieCreateEditVM viewModel);
        Task<Movie> UpdateMovieAsync(int id, MovieCreateEditVM viewModel);
        Task DeleteMovieAsync(int id);
        Task<int> GetNowShowingCountAsync();
        Task<int> GetComingSoonCountAsync();
        Task DeleteImageAsync(int imageId);
        Task UpdateImageOrderAsync(int imageId, int newOrder);
        Task<bool> MovieExistsAsync(int id);
    }
}