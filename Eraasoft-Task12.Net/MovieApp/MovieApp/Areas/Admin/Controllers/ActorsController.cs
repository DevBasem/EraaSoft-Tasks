using Microsoft.AspNetCore.Mvc;
using MovieApp.Areas.Admin;
using MovieApp.Models;
using MovieApp.Services.Interfaces;
using MovieApp.Utilities;
using MovieApp.ViewModels.Admin;
using System;
using System.Threading.Tasks;

namespace MovieApp.Areas.Admin.Controllers
{
    [AdminAreaAttribute]
    public class ActorsController : Controller
    {
        private readonly IActorService _actorService;

        public ActorsController(IActorService actorService)
        {
            _actorService = actorService;
        }

        public async Task<IActionResult> Index()
        {
            var viewModels = await _actorService.GetAllActorsAsync();
            return View(viewModels);
        }

        public async Task<IActionResult> Details(int id)
        {
            var actorViewModel = await _actorService.GetActorDetailsAsync(id);

            if (actorViewModel == null)
            {
                ToastNotification.Error(TempData, "Actor not found");
                return NotFound();
            }

            return View(actorViewModel);
        }

        public IActionResult Create()
        {
            return View(new ActorCreateEditVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ActorCreateEditVM viewModel)
        {
            if (ModelState.IsValid)
            {
                var actor = await _actorService.CreateActorAsync(viewModel);
                ToastNotification.Success(TempData, $"Actor '{actor.FullName}' was created successfully");
                return RedirectToAction(nameof(Index));
            }
            
            ToastNotification.Error(TempData, "Please correct the errors and try again");
            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var actor = await _actorService.GetActorByIdAsync(id);

            if (actor == null)
            {
                ToastNotification.Error(TempData, "Actor not found");
                return NotFound();
            }

            var viewModel = new ActorCreateEditVM
            {
                Id = actor.Id,
                FullName = actor.FullName,
                Bio = actor.Bio,
                ProfileImagePath = actor.ProfileImagePath
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ActorCreateEditVM viewModel)
        {
            if (id != viewModel.Id)
            {
                ToastNotification.Error(TempData, "Invalid actor ID");
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var actor = await _actorService.UpdateActorAsync(id, viewModel);
                    
                    if (actor == null)
                    {
                        ToastNotification.Error(TempData, "Actor not found");
                        return NotFound();
                    }
                    
                    ToastNotification.Success(TempData, $"Actor '{actor.FullName}' was updated successfully");
                }
                catch (Exception)
                {
                    if (!await _actorService.ActorExistsAsync(viewModel.Id))
                    {
                        ToastNotification.Error(TempData, "Actor not found");
                        return NotFound();
                    }
                    else
                    {
                        ToastNotification.Error(TempData, "An error occurred while updating the actor");
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
            var viewModel = await _actorService.GetActorDetailsAsync(id);

            if (viewModel == null)
            {
                ToastNotification.Error(TempData, "Actor not found");
                return NotFound();
            }

            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var actor = await _actorService.GetActorByIdAsync(id);
            
            if (actor != null)
            {
                var actorName = actor.FullName;
                await _actorService.DeleteActorAsync(id);
                ToastNotification.Success(TempData, $"Actor '{actorName}' was deleted successfully");
            }
            else
            {
                ToastNotification.Error(TempData, "Actor not found");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}