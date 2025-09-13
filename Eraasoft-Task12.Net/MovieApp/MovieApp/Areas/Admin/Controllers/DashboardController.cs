using Microsoft.AspNetCore.Mvc;
using MovieApp.Areas.Admin;
using MovieApp.Services.Interfaces;
using System.Threading.Tasks;

namespace MovieApp.Areas.Admin.Controllers
{
    [AdminAreaAttribute]
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = await _dashboardService.GetDashboardDataAsync();
            return View(viewModel);
        }
    }
}