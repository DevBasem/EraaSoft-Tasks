using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieApp.DataAccess;
using MovieApp.Models;
using MovieApp.Utilities;
using MovieApp.ViewModels;
using MovieApp.ViewModels.Admin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MoviesController : Controller
    {
        private readonly MoviesDbContext _context;
        private const int PageSize = 10;

        public MoviesController(MoviesDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int page = 1, string searchTerm = "", string sortField = "Title", string sortOrder = "asc", string statusFilter = "all")
        {
            ViewData["CurrentSearchTerm"] = searchTerm;
            ViewData["CurrentSortField"] = sortField;
            ViewData["CurrentSortOrder"] = sortOrder;
            ViewData["CurrentStatusFilter"] = statusFilter;
            ViewData["CurrentPage"] = page;
            
            // Start with all movies
            var moviesQuery = _context.Movies.Include(m => m.Category).AsQueryable();
            
            // Apply status filter
            if (statusFilter == "nowshowing")
            {
                moviesQuery = moviesQuery.Where(m => m.Status == MovieStatus.NowShowing);
            }
            else if (statusFilter == "comingsoon")
            {
                moviesQuery = moviesQuery.Where(m => m.Status == MovieStatus.ComingSoon);
            }
            
            // Apply search if provided
            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                moviesQuery = moviesQuery.Where(m => 
                    m.Title.ToLower().Contains(searchTerm) || 
                    (m.Category != null && m.Category.Name.ToLower().Contains(searchTerm)) ||
                    m.ReleaseYear.ToString().Contains(searchTerm)
                );
            }
            
            // Count total filtered records for pagination
            var totalMovies = await moviesQuery.CountAsync();
            
            // Apply sorting
            moviesQuery = sortField.ToLower() switch
            {
                "title" => sortOrder.ToLower() == "desc" 
                    ? moviesQuery.OrderByDescending(m => m.Title) 
                    : moviesQuery.OrderBy(m => m.Title),
                "category" => sortOrder.ToLower() == "desc" 
                    ? moviesQuery.OrderByDescending(m => m.Category != null ? m.Category.Name : "Uncategorized") 
                    : moviesQuery.OrderBy(m => m.Category != null ? m.Category.Name : "Uncategorized"),
                "year" => sortOrder.ToLower() == "desc" 
                    ? moviesQuery.OrderByDescending(m => m.ReleaseYear) 
                    : moviesQuery.OrderBy(m => m.ReleaseYear),
                "status" => sortOrder.ToLower() == "desc" 
                    ? moviesQuery.OrderByDescending(m => m.Status) 
                    : moviesQuery.OrderBy(m => m.Status),
                _ => sortOrder.ToLower() == "desc" 
                    ? moviesQuery.OrderByDescending(m => m.Id) 
                    : moviesQuery.OrderBy(m => m.Id)
            };
            
            // Apply pagination
            var movies = await moviesQuery
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .Select(m => new MovieApp.ViewModels.Admin.MovieItemVM
                {
                    Id = m.Id,
                    Title = m.Title,
                    CategoryName = m.Category != null ? m.Category.Name : "Uncategorized",
                    PosterUrl = m.PosterUrl,
                    ReleaseYear = m.ReleaseYear,
                    Status = m.Status
                })
                .ToListAsync();

            // Create a status count summary for the chart
            var nowShowingCount = await _context.Movies.CountAsync(m => m.Status == MovieStatus.NowShowing);
            var comingSoonCount = await _context.Movies.CountAsync(m => m.Status == MovieStatus.ComingSoon);
            
            ViewData["NowShowingCount"] = nowShowingCount;
            ViewData["ComingSoonCount"] = comingSoonCount;
            
            // Create pager
            var pager = new Pager(totalMovies, page, PageSize);
            ViewData["Pager"] = pager;

            return View(movies);
        }

        public async Task<IActionResult> Details(int id)
        {
            var movie = await _context.Movies
                .Include(m => m.Category)
                .Include(m => m.Cinema)
                .Include(m => m.Images)
                .Include(m => m.Actors)
                    .ThenInclude(ma => ma.Actor)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                ToastNotification.Error(TempData, "Movie not found");
                return NotFound();
            }

            var viewModel = new MovieApp.ViewModels.Admin.MovieDetailsVM
            {
                Id = movie.Id,
                Title = movie.Title,
                Description = movie.Description,
                ReleaseYear = movie.ReleaseYear,
                DurationMinutes = movie.DurationMinutes,
                PosterUrl = movie.PosterUrl,
                Price = movie.Price,
                Status = movie.Status,
                TotalTickets = movie.TotalTickets,
                ReservedTickets = movie.ReservedTickets,
                AvailableTickets = movie.TotalTickets - movie.ReservedTickets,
                TrailerUrl = movie.TrailerUrl,
                
                CategoryName = movie.Category?.Name,
                CategoryId = movie.CategoryId,
                
                CinemaName = movie.Cinema?.Name,
                CinemaId = movie.Cinema?.Id,
                CinemaLocation = movie.Cinema?.Location,
                CinemaCity = movie.Cinema?.City,
                CinemaAddress = movie.Cinema?.Address,
                CinemaPhone = movie.Cinema?.PhoneNumber,
                CinemaEmail = movie.Cinema?.Email,
                
                Images = movie.Images?.OrderBy(i => i.Order).ToList(),
                
                Actors = movie.Actors?.Select(ma => new MovieApp.ViewModels.Admin.ActorVM
                {
                    Id = ma.Actor.Id,
                    FullName = ma.Actor.FullName,
                    ProfileImagePath = ma.Actor.ProfileImagePath
                }).ToList()
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Create()
        {
            var viewModel = new MovieApp.ViewModels.Admin.MovieCreateEditVM
            {
                Categories = await _context.Categories
                    .OrderBy(c => c.Name)
                    .Select(c => new MovieApp.ViewModels.Admin.CategoryVM 
                    { 
                        Id = c.Id, 
                        Name = c.Name 
                    })
                    .ToListAsync(),
                
                Cinemas = await _context.Cinemas
                    .OrderBy(c => c.Name)
                    .Select(c => new MovieApp.ViewModels.Admin.CinemaVM 
                    { 
                        Id = c.Id, 
                        Name = c.Name,
                        Location = c.Location,
                        City = c.City
                    })
                    .ToListAsync(),
                
                AllActors = await _context.Actors
                    .OrderBy(a => a.FullName)
                    .Select(a => new MovieApp.ViewModels.Admin.ActorSelectVM
                    {
                        Id = a.Id,
                        FullName = a.FullName
                    })
                    .ToListAsync()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MovieApp.ViewModels.Admin.MovieCreateEditVM viewModel)
        {
            if (ModelState.IsValid)
            {
                var movie = new Movie
                {
                    Title = viewModel.Title,
                    Description = viewModel.Description,
                    ReleaseYear = viewModel.ReleaseYear,
                    DurationMinutes = viewModel.DurationMinutes,
                    PosterUrl = viewModel.PosterUrl,
                    Price = viewModel.Price,
                    Status = viewModel.Status,
                    TotalTickets = viewModel.TotalTickets,
                    ReservedTickets = viewModel.ReservedTickets,
                    CategoryId = viewModel.CategoryId,
                    CinemaId = viewModel.CinemaId,
                    TrailerUrl = viewModel.TrailerUrl
                };

                _context.Movies.Add(movie);
                await _context.SaveChangesAsync();

                // Process poster file if uploaded
                if (viewModel.PosterFile != null)
                {
                    string fileName = await SaveFile(viewModel.PosterFile);
                    movie.PosterUrl = fileName;
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }

                // Process movie gallery images if uploaded
                if (viewModel.MovieImageFiles != null && viewModel.MovieImageFiles.Any())
                {
                    int order = 1;
                    foreach (var imageFile in viewModel.MovieImageFiles)
                    {
                        if (imageFile != null && imageFile.Length > 0)
                        {
                            string filePath = await SaveFile(imageFile);
                            
                            var movieImage = new MovieImage
                            {
                                MovieId = movie.Id,
                                Path = filePath,
                                Order = order++
                            };
                            
                            _context.MovieImages.Add(movieImage);
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                // Process selected actors
                if (viewModel.SelectedActorIds != null && viewModel.SelectedActorIds.Any())
                {
                    foreach (var actorId in viewModel.SelectedActorIds)
                    {
                        var movieActor = new MovieActor
                        {
                            MovieId = movie.Id,
                            ActorId = actorId
                        };
                        
                        _context.MovieActors.Add(movieActor);
                    }
                    await _context.SaveChangesAsync();
                }

                ToastNotification.Success(TempData, $"Movie '{movie.Title}' was created successfully");
                return RedirectToAction(nameof(Index));
            }

            // If we got this far, something failed - reload form
            viewModel.Categories = await _context.Categories.OrderBy(c => c.Name)
                .Select(c => new MovieApp.ViewModels.Admin.CategoryVM { Id = c.Id, Name = c.Name })
                .ToListAsync();
                
            viewModel.Cinemas = await _context.Cinemas.OrderBy(c => c.Name)
                .Select(c => new MovieApp.ViewModels.Admin.CinemaVM { Id = c.Id, Name = c.Name })
                .ToListAsync();
                
            viewModel.AllActors = await _context.Actors.OrderBy(a => a.FullName)
                .Select(a => new MovieApp.ViewModels.Admin.ActorSelectVM { Id = a.Id, FullName = a.FullName })
                .ToListAsync();
            
            ToastNotification.Error(TempData, "Please correct the errors and try again");
            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var movie = await _context.Movies
                .Include(m => m.Actors)
                .Include(m => m.Images)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                ToastNotification.Error(TempData, "Movie not found");
                return NotFound();
            }

            // Extract actor IDs to a separate list for client-side evaluation
            var selectedActorIds = movie.Actors?.Select(ma => ma.ActorId).ToList() ?? new List<int>();

            var viewModel = new MovieApp.ViewModels.Admin.MovieCreateEditVM
            {
                Id = movie.Id,
                Title = movie.Title,
                Description = movie.Description,
                ReleaseYear = movie.ReleaseYear,
                DurationMinutes = movie.DurationMinutes,
                PosterUrl = movie.PosterUrl,
                Price = movie.Price,
                Status = movie.Status,
                TotalTickets = movie.TotalTickets,
                ReservedTickets = movie.ReservedTickets,
                CategoryId = movie.CategoryId,
                CinemaId = movie.CinemaId,
                SelectedActorIds = selectedActorIds,
                MovieImages = movie.Images?.OrderBy(i => i.Order).ToList() ?? new List<MovieImage>(),
                TrailerUrl = movie.TrailerUrl,
                
                Categories = await _context.Categories
                    .OrderBy(c => c.Name)
                    .Select(c => new MovieApp.ViewModels.Admin.CategoryVM { Id = c.Id, Name = c.Name })
                    .ToListAsync(),
                
                Cinemas = await _context.Cinemas
                    .OrderBy(c => c.Name)
                    .Select(c => new MovieApp.ViewModels.Admin.CinemaVM 
                    { 
                        Id = c.Id, 
                        Name = c.Name, 
                        Location = c.Location, 
                        City = c.City 
                    })
                    .ToListAsync(),
            };

            // Get all actors separately
            var allActors = await _context.Actors
                .OrderBy(a => a.FullName)
                .Select(a => new MovieApp.ViewModels.Admin.ActorSelectVM
                {
                    Id = a.Id,
                    FullName = a.FullName
                })
                .ToListAsync();

            // Set the IsSelected property on the client side
            foreach (var actor in allActors)
            {
                actor.IsSelected = selectedActorIds.Contains(actor.Id);
            }

            viewModel.AllActors = allActors;

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MovieApp.ViewModels.Admin.MovieCreateEditVM viewModel)
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
                    var movie = await _context.Movies
                        .Include(m => m.Actors)
                        .FirstOrDefaultAsync(m => m.Id == id);
                    
                    if (movie == null)
                    {
                        ToastNotification.Error(TempData, "Movie not found");
                        return NotFound();
                    }

                    // Update basic movie properties
                    movie.Title = viewModel.Title;
                    movie.Description = viewModel.Description;
                    movie.ReleaseYear = viewModel.ReleaseYear;
                    movie.DurationMinutes = viewModel.DurationMinutes;
                    movie.Price = viewModel.Price;
                    movie.Status = viewModel.Status;
                    movie.TotalTickets = viewModel.TotalTickets;
                    movie.ReservedTickets = viewModel.ReservedTickets;
                    movie.CategoryId = viewModel.CategoryId;
                    movie.CinemaId = viewModel.CinemaId;
                    movie.TrailerUrl = viewModel.TrailerUrl;

                    // Process poster file if uploaded
                    if (viewModel.PosterFile != null && viewModel.PosterFile.Length > 0)
                    {
                        string fileName = await SaveFile(viewModel.PosterFile);
                        movie.PosterUrl = fileName;
                    }
                    else if (!string.IsNullOrEmpty(viewModel.PosterUrl) && viewModel.PosterUrl != movie.PosterUrl)
                    {
                        // If a new URL was provided and no file was uploaded
                        movie.PosterUrl = viewModel.PosterUrl;
                    }

                    // Process movie gallery images if uploaded
                    if (viewModel.MovieImageFiles != null && viewModel.MovieImageFiles.Any(f => f != null && f.Length > 0))
                    {
                        // Get the highest order number currently in the database
                        var highestOrder = await _context.MovieImages
                            .Where(mi => mi.MovieId == id)
                            .Select(mi => mi.Order)
                            .DefaultIfEmpty(0)
                            .MaxAsync();

                        int order = highestOrder + 1;
                        
                        foreach (var imageFile in viewModel.MovieImageFiles)
                        {
                            if (imageFile != null && imageFile.Length > 0)
                            {
                                string filePath = await SaveFile(imageFile);
                                
                                var movieImage = new MovieImage
                                {
                                    MovieId = movie.Id,
                                    Path = filePath,
                                    Order = order++
                                };
                                
                                _context.MovieImages.Add(movieImage);
                            }
                        }
                    }

                    // Update the actors - first remove all existing associations
                    if (movie.Actors != null)
                    {
                        _context.MovieActors.RemoveRange(movie.Actors);
                    }
                    
                    // Now add the selected actors
                    if (viewModel.SelectedActorIds != null && viewModel.SelectedActorIds.Any())
                    {
                        foreach (var actorId in viewModel.SelectedActorIds)
                        {
                            _context.MovieActors.Add(new MovieActor
                            {
                                MovieId = movie.Id,
                                ActorId = actorId
                            });
                        }
                    }
                    
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                    
                    ToastNotification.Success(TempData, $"Movie '{movie.Title}' was updated successfully");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(viewModel.Id))
                    {
                        ToastNotification.Error(TempData, "Movie not found");
                        return NotFound();
                    }
                    else
                    {
                        ToastNotification.Error(TempData, "An error occurred while updating the movie");
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            
            // If we got this far, something failed - reload form
            viewModel.Categories = await _context.Categories.OrderBy(c => c.Name)
                .Select(c => new MovieApp.ViewModels.Admin.CategoryVM { Id = c.Id, Name = c.Name })
                .ToListAsync();
                
            viewModel.Cinemas = await _context.Cinemas.OrderBy(c => c.Name)
                .Select(c => new MovieApp.ViewModels.Admin.CinemaVM { Id = c.Id, Name = c.Name })
                .ToListAsync();
            
            var allActors = await _context.Actors.OrderBy(a => a.FullName).ToListAsync();
            viewModel.AllActors = allActors.Select(a => new MovieApp.ViewModels.Admin.ActorSelectVM
            {
                Id = a.Id,
                FullName = a.FullName,
                IsSelected = viewModel.SelectedActorIds.Contains(a.Id)
            }).ToList();
            
            ToastNotification.Error(TempData, "Please correct the errors and try again");
            return View(viewModel);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var movie = await _context.Movies
                .Include(m => m.Category)
                .Include(m => m.Cinema)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                ToastNotification.Error(TempData, "Movie not found");
                return NotFound();
            }

            // Return the full Movie entity instead of MovieItemVM
            return View(movie);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movies
                .Include(m => m.Images)
                .Include(m => m.Actors)
                .FirstOrDefaultAsync(m => m.Id == id);
                
            if (movie != null)
            {
                // Store the movie title for success message
                var movieTitle = movie.Title;
                
                // Remove movie actors relations
                if (movie.Actors != null && movie.Actors.Any())
                {
                    _context.MovieActors.RemoveRange(movie.Actors);
                }
                
                // Remove movie images
                if (movie.Images != null && movie.Images.Any())
                {
                    _context.MovieImages.RemoveRange(movie.Images);
                }
                
                _context.Movies.Remove(movie);
                await _context.SaveChangesAsync();
                
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
            var image = await _context.MovieImages.FindAsync(imageId);
            if (image == null)
            {
                return NotFound();
            }

            _context.MovieImages.Remove(image);
            await _context.SaveChangesAsync();
            
            ToastNotification.Success(TempData, "Image was deleted successfully");
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateImageOrder(int imageId, int newOrder)
        {
            var image = await _context.MovieImages.FindAsync(imageId);
            if (image == null)
            {
                return NotFound();
            }

            // Get all images for this movie
            var movieImages = await _context.MovieImages
                .Where(mi => mi.MovieId == image.MovieId)
                .OrderBy(mi => mi.Order)
                .ToListAsync();
            
            // If trying to move beyond the bounds, limit to valid range
            if (newOrder < 1)
            {
                newOrder = 1;
            }
            else if (newOrder > movieImages.Count)
            {
                newOrder = movieImages.Count;
            }
            
            int oldOrder = image.Order;
            
            // Update orders accordingly
            if (newOrder < oldOrder)
            {
                // Moving up - increment orders for images in between
                foreach (var img in movieImages.Where(mi => mi.Order >= newOrder && mi.Order < oldOrder))
                {
                    img.Order++;
                }
            }
            else if (newOrder > oldOrder)
            {
                // Moving down - decrement orders for images in between
                foreach (var img in movieImages.Where(mi => mi.Order <= newOrder && mi.Order > oldOrder))
                {
                    img.Order--;
                }
            }
            
            // Set the new order for the target image
            image.Order = newOrder;
            
            await _context.SaveChangesAsync();
            
            ToastNotification.Success(TempData, "Image order updated successfully");
            return Ok();
        }

        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.Id == id);
        }

        private async Task<string> SaveFile(Microsoft.AspNetCore.Http.IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return null;
            }

            // Define the uploads directory
            var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            
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
            return $"/uploads/{uniqueFileName}";
        }
    }
}