using Microsoft.AspNetCore.Mvc;
using MovieApp.Areas.Admin;
using MovieApp.Models;
using MovieApp.Services;
using MovieApp.Services.Interfaces;
using MovieApp.Utilities;
using MovieApp.ViewModels;
using MovieApp.ViewModels.Admin;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApp.Areas.Admin.Controllers
{
    [AdminAreaAttribute]
    public class MoviesController : Controller
    {
        private readonly IMovieService _movieService;
        private const int PageSize = 10;

        public MoviesController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        public async Task<IActionResult> Index(int page = 1, string searchTerm = "", string sortField = "Title", string sortOrder = "asc", string statusFilter = "all")
        {
            ViewData["CurrentSearchTerm"] = searchTerm;
            ViewData["CurrentSortField"] = sortField;
            ViewData["CurrentSortOrder"] = sortOrder;
            ViewData["CurrentStatusFilter"] = statusFilter;
            ViewData["CurrentPage"] = page;
            
            var pagedMovies = await _movieService.GetPagedMoviesAsync(page, searchTerm, sortField, sortOrder, statusFilter, PageSize);
            var nowShowingCount = await _movieService.GetNowShowingCountAsync();
            var comingSoonCount = await _movieService.GetComingSoonCountAsync();
            
            ViewData["NowShowingCount"] = nowShowingCount;
            ViewData["ComingSoonCount"] = comingSoonCount;
            
            // Create pager from the total count in the collection
            var pager = new MovieApp.ViewModels.Pager(pagedMovies.TotalCount, page, PageSize);
            ViewData["Pager"] = pager;

            return View(pagedMovies);
        }

        public async Task<IActionResult> Details(int id)
        {
            var viewModel = await _movieService.GetMovieDetailsAsync(id);

            if (viewModel == null)
            {
                ToastNotification.Error(TempData, "Movie not found");
                return NotFound();
            }

            return View(viewModel);
        }

        public async Task<IActionResult> Create()
        {
            var viewModel = await _movieService.PrepareMovieViewModelForCreateAsync();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MovieCreateEditVM viewModel)
        {
            if (ModelState.IsValid)
            {
                var movie = await _movieService.CreateMovieAsync(viewModel);
                ToastNotification.Success(TempData, $"Movie '{movie.Title}' was created successfully");
                return RedirectToAction(nameof(Index));
            }

            viewModel = await _movieService.PrepareMovieViewModelForCreateAsync();
            ToastNotification.Error(TempData, "Please correct the errors and try again");
            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var viewModel = await _movieService.PrepareMovieViewModelForEditAsync(id);

            if (viewModel == null)
            {
                ToastNotification.Error(TempData, "Movie not found");
                return NotFound();
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MovieCreateEditVM viewModel)
        {
            if (id != viewModel.Id)
            {
                ToastNotification.Error(TempData, "Invalid movie ID");
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var movie = await _movieService.UpdateMovieAsync(id, viewModel);
                    
                    if (movie == null)
                    {
                        ToastNotification.Error(TempData, "Movie not found");
                        return NotFound();
                    }
                    
                    ToastNotification.Success(TempData, $"Movie '{movie.Title}' was updated successfully");
                }
                catch (Exception ex)
                {
                    if (!await _movieService.MovieExistsAsync(viewModel.Id))
                    {
                        ToastNotification.Error(TempData, "Movie not found");
                        return NotFound();
                    }
                    else
                    {
                        ToastNotification.Error(TempData, $"An error occurred while updating the movie: {ex.Message}");
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            
            viewModel = await _movieService.PrepareMovieViewModelForEditAsync(id);
            ToastNotification.Error(TempData, "Please correct the errors and try again");
            return View(viewModel);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var movie = await _movieService.GetMovieWithDetailsAsync(id);

            if (movie == null)
            {
                ToastNotification.Error(TempData, "Movie not found");
                return NotFound();
            }

            return View(movie);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _movieService.GetMovieWithDetailsAsync(id);
                
            if (movie != null)
            {
                var movieTitle = movie.Title;
                await _movieService.DeleteMovieAsync(id);
                ToastNotification.Success(TempData, $"Movie '{movieTitle}' was deleted successfully");
            }
            else
            {
                ToastNotification.Error(TempData, "Movie not found");
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteImage(int imageId)
        {
            await _movieService.DeleteImageAsync(imageId);
            ToastNotification.Success(TempData, "Image was deleted successfully");
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateImageOrder(int imageId, int newOrder)
        {
            await _movieService.UpdateImageOrderAsync(imageId, newOrder);
            ToastNotification.Success(TempData, "Image order updated successfully");
            return Ok();
        }
    }
}