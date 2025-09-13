using MovieApp.ViewModels.Admin;

namespace MovieApp.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardVM> GetDashboardDataAsync();
    }
}