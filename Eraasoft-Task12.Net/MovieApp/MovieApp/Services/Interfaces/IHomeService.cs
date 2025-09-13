using MovieApp.ViewModels;

namespace MovieApp.Services.Interfaces
{
    public interface IHomeService
    {
        Task<HomeIndexVM> GetHomePageDataAsync();
    }
}