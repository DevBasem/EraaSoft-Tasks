using MovieApp.Models;
using MovieApp.ViewModels;

namespace MovieApp.Services.Interfaces
{
    public interface IPublicMovieService
    {
        Task<MoviesIndexVM> GetMoviesIndexAsync(int? page, int? categoryId, int? status, string? sort);
        Task<MovieDetailsVM> GetMovieDetailsAsync(int id);
    }
}