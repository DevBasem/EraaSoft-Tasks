using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieApp.DataAccess;
using MovieApp.Models;
using MovieApp.Utilities;
using MovieApp.ViewModels;
using MovieApp.ViewModels.Admin;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ActorsController : Controller
    {
        private readonly MoviesDbContext _context;

        public ActorsController(MoviesDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var actors = await _context.Actors
                .Include(a => a.Movies)
                    .ThenInclude(ma => ma.Movie)
                .Select(a => new ViewModels.Admin.ActorVM
                {
                    Id = a.Id,
                    FullName = a.FullName,
                    ProfileImagePath = a.ProfileImagePath,
                    Bio = a.Bio,
                    MovieCount = a.Movies.Count
                })
                .OrderBy(a => a.FullName)
                .ToListAsync();

            return View(actors);
        }

        public async Task<IActionResult> Details(int id)
        {
            var actor = await _context.Actors
                .Include(a => a.Movies)
                    .ThenInclude(ma => ma.Movie)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (actor == null)
            {
                ToastNotification.Error(TempData, "Actor not found");
                return NotFound();
            }

            // Convert to a custom ViewModel to avoid conflicts
            var actorViewModel = new ViewModels.Admin.ActorVM
            {
                Id = actor.Id,
                FullName = actor.FullName,
                ProfileImagePath = actor.ProfileImagePath,
                Bio = actor.Bio,
                MovieCount = actor.Movies.Count
            };

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
                var actor = new Actor
                {
                    FullName = viewModel.FullName,
                    Bio = viewModel.Bio,
                    ProfileImagePath = viewModel.ProfileImagePath
                };

                _context.Actors.Add(actor);
                await _context.SaveChangesAsync();

                // Process profile image file if uploaded
                if (viewModel.ProfileImageFile != null && viewModel.ProfileImageFile.Length > 0)
                {
                    string fileName = await SaveFile(viewModel.ProfileImageFile);
                    actor.ProfileImagePath = fileName;
                    _context.Update(actor);
                    await _context.SaveChangesAsync();
                }
                
                ToastNotification.Success(TempData, $"Actor '{actor.FullName}' was created successfully");
                return RedirectToAction(nameof(Index));
            }
            
            ToastNotification.Error(TempData, "Please correct the errors and try again");
            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var actor = await _context.Actors.FindAsync(id);

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
                    var actor = await _context.Actors.FindAsync(id);

                    if (actor == null)
                    {
                        ToastNotification.Error(TempData, "Actor not found");
                        return NotFound();
                    }

                    actor.FullName = viewModel.FullName;
                    actor.Bio = viewModel.Bio;
                    
                    // Process profile image file if uploaded
                    if (viewModel.ProfileImageFile != null && viewModel.ProfileImageFile.Length > 0)
                    {
                        string fileName = await SaveFile(viewModel.ProfileImageFile);
                        actor.ProfileImagePath = fileName;
                    }
                    else if (!string.IsNullOrEmpty(viewModel.ProfileImagePath) && viewModel.ProfileImagePath != actor.ProfileImagePath)
                    {
                        // If a new URL was provided and no file was uploaded
                        actor.ProfileImagePath = viewModel.ProfileImagePath;
                    }

                    _context.Update(actor);
                    await _context.SaveChangesAsync();
                    
                    ToastNotification.Success(TempData, $"Actor '{actor.FullName}' was updated successfully");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActorExists(viewModel.Id))
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
            var actor = await _context.Actors
                .Include(a => a.Movies)
                    .ThenInclude(ma => ma.Movie)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (actor == null)
            {
                ToastNotification.Error(TempData, "Actor not found");
                return NotFound();
            }

            var viewModel = new ViewModels.Admin.ActorVM
            {
                Id = actor.Id,
                FullName = actor.FullName,
                ProfileImagePath = actor.ProfileImagePath,
                Bio = actor.Bio,
                MovieCount = actor.Movies.Count
            };

            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var actor = await _context.Actors.FindAsync(id);
            
            if (actor != null)
            {
                var actorName = actor.FullName;
                _context.Actors.Remove(actor);
                await _context.SaveChangesAsync();
                ToastNotification.Success(TempData, $"Actor '{actorName}' was deleted successfully");
            }
            else
            {
                ToastNotification.Error(TempData, "Actor not found");
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ActorExists(int id)
        {
            return _context.Actors.Any(e => e.Id == id);
        }
        
        private async Task<string> SaveFile(Microsoft.AspNetCore.Http.IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return null;
            }

            // Define the uploads directory
            var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "actors");
            
            // Create directory if it doesn't exist
            if (!Directory.Exists(uploadsDir))
            {
                Directory.CreateDirectory(uploadsDir);
            }
            
            // Create a unique filename
            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsDir, uniqueFileName);
            
            // Save the file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            
            // Return the relative path that will be stored in the database
            return $"/uploads/actors/{uniqueFileName}";
        }
    }
}