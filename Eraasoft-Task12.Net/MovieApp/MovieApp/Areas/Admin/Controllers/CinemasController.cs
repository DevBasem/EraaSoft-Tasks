using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieApp.DataAccess;
using MovieApp.Models;
using MovieApp.ViewModels;
using MovieApp.ViewModels.Admin;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CinemasController : Controller
    {
        private readonly MoviesDbContext _context;

        public CinemasController(MoviesDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var cinemas = await _context.Cinemas
                .Include(c => c.Movies)
                .Select(c => new ViewModels.Admin.CinemaVM
                {
                    Id = c.Id,
                    Name = c.Name,
                    Location = c.Location,
                    City = c.City,
                    LogoPath = c.LogoPath,
                    MovieCount = c.Movies.Count()
                })
                .OrderBy(c => c.Name)
                .ToListAsync();

            return View(cinemas);
        }

        public async Task<IActionResult> Details(int id)
        {
            var cinema = await _context.Cinemas
                .Include(c => c.Movies)
                    .ThenInclude(m => m.Category)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cinema == null)
            {
                return NotFound();
            }

            // Create a custom view model that uses the Admin.CinemaVM
            var viewModel = new ViewModels.Admin.CinemaVM
            {
                Id = cinema.Id,
                Name = cinema.Name,
                Location = cinema.Location,
                City = cinema.City,
                LogoPath = cinema.LogoPath,
                MovieCount = cinema.Movies.Count()
            };

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
                var cinema = new Cinema
                {
                    Name = viewModel.Name,
                    Location = viewModel.Location,
                    Address = viewModel.Address,
                    City = viewModel.City,
                    PhoneNumber = viewModel.PhoneNumber,
                    Email = viewModel.Email,
                    LogoPath = viewModel.LogoPath
                };

                _context.Cinemas.Add(cinema);
                await _context.SaveChangesAsync();
                
                // Process logo file if uploaded
                if (viewModel.LogoFile != null && viewModel.LogoFile.Length > 0)
                {
                    string fileName = await SaveFile(viewModel.LogoFile);
                    cinema.LogoPath = fileName;
                    _context.Update(cinema);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var cinema = await _context.Cinemas.FindAsync(id);

            if (cinema == null)
            {
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
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var cinema = await _context.Cinemas.FindAsync(id);

                    if (cinema == null)
                    {
                        return NotFound();
                    }

                    cinema.Name = viewModel.Name;
                    cinema.Location = viewModel.Location;
                    cinema.Address = viewModel.Address;
                    cinema.City = viewModel.City;
                    cinema.PhoneNumber = viewModel.PhoneNumber;
                    cinema.Email = viewModel.Email;
                    
                    // Process logo file if uploaded
                    if (viewModel.LogoFile != null && viewModel.LogoFile.Length > 0)
                    {
                        string fileName = await SaveFile(viewModel.LogoFile);
                        cinema.LogoPath = fileName;
                    }
                    else if (!string.IsNullOrEmpty(viewModel.LogoPath) && viewModel.LogoPath != cinema.LogoPath)
                    {
                        // If a new URL was provided and no file was uploaded
                        cinema.LogoPath = viewModel.LogoPath;
                    }

                    _context.Update(cinema);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CinemaExists(viewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(viewModel);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var cinema = await _context.Cinemas
                .Include(c => c.Movies)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cinema == null)
            {
                return NotFound();
            }

            var viewModel = new ViewModels.Admin.CinemaVM
            {
                Id = cinema.Id,
                Name = cinema.Name,
                Location = cinema.Location,
                City = cinema.City,
                LogoPath = cinema.LogoPath,
                MovieCount = cinema.Movies.Count()
            };

            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cinema = await _context.Cinemas.FindAsync(id);
            
            if (cinema != null)
            {
                _context.Cinemas.Remove(cinema);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CinemaExists(int id)
        {
            return _context.Cinemas.Any(e => e.Id == id);
        }
        
        private async Task<string> SaveFile(Microsoft.AspNetCore.Http.IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return null;
            }

            // Define the uploads directory
            var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "cinemas");
            
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
            return $"/uploads/cinemas/{uniqueFileName}";
        }
    }
}