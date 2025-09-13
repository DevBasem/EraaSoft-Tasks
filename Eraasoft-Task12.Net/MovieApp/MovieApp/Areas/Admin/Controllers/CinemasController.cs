using Microsoft.AspNetCore.Mvc;
using MovieApp.Areas.Admin;
using MovieApp.Services.Interfaces;
using MovieApp.Utilities;
using MovieApp.ViewModels.Admin;
using System;
using System.Threading.Tasks;

namespace MovieApp.Areas.Admin.Controllers
{
    [AdminAreaAttribute]
    public class CinemasController : Controller
    {
        private readonly ICinemaService _cinemaService;

        public CinemasController(ICinemaService cinemaService)
        {
            _cinemaService = cinemaService;
        }

        public async Task<IActionResult> Index()
        {
            var viewModels = await _cinemaService.GetAllCinemasAsync();
            return View(viewModels);
        }

        public async Task<IActionResult> Details(int id)
        {
            var viewModel = await _cinemaService.GetCinemaDetailsAsync(id);

            if (viewModel == null)
            {
                ToastNotification.Error(TempData, "Cinema not found");
                return NotFound();
            }

            return View(viewModel);
        }

        public IActionResult Create()
        {
            return View(new CinemaCreateEditVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CinemaCreateEditVM viewModel)
        {
            if (ModelState.IsValid)
            {
                var cinema = await _cinemaService.CreateCinemaAsync(viewModel);
                ToastNotification.Success(TempData, $"Cinema '{cinema.Name}' was created successfully");
                return RedirectToAction(nameof(Index));
            }
            
            ToastNotification.Error(TempData, "Please correct the errors and try again");
            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var cinema = await _cinemaService.GetCinemaByIdAsync(id);

            if (cinema == null)
            {
                ToastNotification.Error(TempData, "Cinema not found");
                return NotFound();
            }

            var viewModel = new CinemaCreateEditVM
            {
                Id = cinema.Id,
                Name = cinema.Name,
                Location = cinema.Location,
                Address = cinema.Address,
                City = cinema.City,
                PhoneNumber = cinema.PhoneNumber,
                Email = cinema.Email,
                LogoPath = cinema.LogoPath
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CinemaCreateEditVM viewModel)
        {
            if (id != viewModel.Id)
            {
                ToastNotification.Error(TempData, "Invalid cinema ID");
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var cinema = await _cinemaService.UpdateCinemaAsync(id, viewModel);

                    if (cinema == null)
                    {
                        ToastNotification.Error(TempData, "Cinema not found");
                        return NotFound();
                    }

                    ToastNotification.Success(TempData, $"Cinema '{cinema.Name}' was updated successfully");
                }
                catch (Exception)
                {
                    if (!await _cinemaService.CinemaExistsAsync(viewModel.Id))
                    {
                        ToastNotification.Error(TempData, "Cinema not found");
                        return NotFound();
                    }
                    else
                    {
                        ToastNotification.Error(TempData, "An error occurred while updating the cinema");
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            
            ToastNotification.Error(TempData, "Please correct the errors and try again");
            return View(viewModel);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var viewModel = await _cinemaService.GetCinemaDetailsAsync(id);

            if (viewModel == null)
            {
                ToastNotification.Error(TempData, "Cinema not found");
                return NotFound();
            }

            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cinema = await _cinemaService.GetCinemaByIdAsync(id);
            
            if (cinema != null)
            {
                var cinemaName = cinema.Name;
                await _cinemaService.DeleteCinemaAsync(id);
                ToastNotification.Success(TempData, $"Cinema '{cinemaName}' was deleted successfully");
            }
            else
            {
                ToastNotification.Error(TempData, "Cinema not found");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}