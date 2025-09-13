using Microsoft.EntityFrameworkCore;
using MovieApp.Models;
using MovieApp.Repositories.IRepositories;
using MovieApp.Services.Interfaces;
using MovieApp.ViewModels.Admin;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MovieApp.Services
{
    public class MovieItemVMCollection : List<MovieItemVM>
    {
        public int TotalCount { get; set; }
    }

    public class MovieService : IMovieService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorageService _fileStorageService;
        private const int DefaultPageSize = 10;

        public MovieService(IUnitOfWork unitOfWork, IFileStorageService fileStorageService)
        {
            _unitOfWork = unitOfWork;
            _fileStorageService = fileStorageService;
        }

        public async Task<MovieItemVMCollection> GetPagedMoviesAsync(
            int page = 1,
            string searchTerm = "",
            string sortField = "Title",
            string sortOrder = "asc",
            string statusFilter = "all",
            int pageSize = 10)
        {
            pageSize = pageSize <= 0 ? DefaultPageSize : pageSize;

            Expression<Func<Movie, object>>[] includes = { m => m.Category };
            var allMovies = await _unitOfWork.Movies.GetAllAsync(includes: includes);
            
            // Filter by status
            if (statusFilter == "nowshowing")
            {
                allMovies = allMovies.Where(m => m.Status == MovieStatus.NowShowing).ToList();
            }
            else if (statusFilter == "comingsoon")
            {
                allMovies = allMovies.Where(m => m.Status == MovieStatus.ComingSoon).ToList();
            }
            
            // Search
            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                allMovies = allMovies.Where(m => 
                    m.Title.ToLower().Contains(searchTerm) || 
                    (m.Category != null && m.Category.Name.ToLower().Contains(searchTerm)) ||
                    m.ReleaseYear.ToString().Contains(searchTerm)
                ).ToList();
            }
            
            // Sort
            allMovies = ApplySorting(allMovies, sortField, sortOrder);
            
            // Get the total count before pagination
            int totalItems = allMovies.Count;
            
            // Create paginated items
            var pagedItems = allMovies
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new MovieItemVM
                {
                    Id = m.Id,
                    Title = m.Title,
                    CategoryName = m.Category != null ? m.Category.Name : "Uncategorized",
                    PosterUrl = m.PosterUrl,
                    ReleaseYear = m.ReleaseYear,
                    Status = m.Status
                })
                .ToList();

            // Create a collection with the total count
            var result = new MovieItemVMCollection();
            result.AddRange(pagedItems);
            result.TotalCount = totalItems;
            
            return result;
        }

        public async Task<Movie> GetMovieByIdAsync(int id)
        {
            return await _unitOfWork.Movies.GetByIdAsync(id);
        }

        public async Task<Movie> GetMovieWithDetailsAsync(int id)
        {
            return await _unitOfWork.Movies.GetMovieWithDetailsAsync(id);
        }

        public async Task<MovieDetailsVM> GetMovieDetailsAsync(int id)
        {
            var movie = await _unitOfWork.Movies.GetMovieWithDetailsAsync(id);
            
            if (movie == null)
                return null;

            return new MovieDetailsVM
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
                
                Actors = movie.Actors?.Select(ma => new ActorVM
                {
                    Id = ma.Actor.Id,
                    FullName = ma.Actor.FullName,
                    ProfileImagePath = ma.Actor.ProfileImagePath
                }).ToList()
            };
        }

        public async Task<MovieCreateEditVM> PrepareMovieViewModelForCreateAsync()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync(orderBy: q => q.OrderBy(c => c.Name));
            var cinemas = await _unitOfWork.Cinemas.GetAllAsync(orderBy: q => q.OrderBy(c => c.Name));
            var actors = await _unitOfWork.Actors.GetAllAsync(orderBy: q => q.OrderBy(a => a.FullName));
            
            return new MovieCreateEditVM
            {
                Categories = categories.Select(c => new CategoryVM 
                { 
                    Id = c.Id, 
                    Name = c.Name 
                }).ToList(),
                
                Cinemas = cinemas.Select(c => new CinemaVM 
                { 
                    Id = c.Id, 
                    Name = c.Name,
                    Location = c.Location,
                    City = c.City
                }).ToList(),
                
                AllActors = actors.Select(a => new ActorSelectVM
                {
                    Id = a.Id,
                    FullName = a.FullName
                }).ToList()
            };
        }

        public async Task<MovieCreateEditVM> PrepareMovieViewModelForEditAsync(int id)
        {
            Expression<Func<Movie, object>>[] includes = { m => m.Actors, m => m.Images };
            var movie = await _unitOfWork.Movies.GetFirstOrDefaultAsync(
                m => m.Id == id,
                includes: includes);

            if (movie == null)
                return null;

            var selectedActorIds = movie.Actors?.Select(ma => ma.ActorId).ToList() ?? new List<int>();

            var categories = await _unitOfWork.Categories.GetAllAsync(orderBy: q => q.OrderBy(c => c.Name));
            var cinemas = await _unitOfWork.Cinemas.GetAllAsync(orderBy: q => q.OrderBy(c => c.Name));
            var actors = await _unitOfWork.Actors.GetAllAsync(orderBy: q => q.OrderBy(a => a.FullName));

            var viewModel = new MovieCreateEditVM
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
                
                Categories = categories.Select(c => new CategoryVM { Id = c.Id, Name = c.Name }).ToList(),
                
                Cinemas = cinemas.Select(c => new CinemaVM 
                { 
                    Id = c.Id, 
                    Name = c.Name, 
                    Location = c.Location, 
                    City = c.City 
                }).ToList(),
                
                AllActors = actors.Select(a => new ActorSelectVM
                {
                    Id = a.Id,
                    FullName = a.FullName,
                    IsSelected = selectedActorIds.Contains(a.Id)
                }).ToList()
            };

            return viewModel;
        }

        public async Task<Movie> CreateMovieAsync(MovieCreateEditVM viewModel)
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

            await _unitOfWork.Movies.AddAsync(movie);
            await _unitOfWork.SaveChangesAsync();

            if (viewModel.PosterFile != null)
            {
                string fileName = await _fileStorageService.SaveFileAsync(viewModel.PosterFile);
                movie.PosterUrl = fileName;
                _unitOfWork.Movies.Update(movie);
                await _unitOfWork.SaveChangesAsync();
            }

            if (viewModel.MovieImageFiles != null && viewModel.MovieImageFiles.Any())
            {
                int order = 1;
                foreach (var imageFile in viewModel.MovieImageFiles)
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        string filePath = await _fileStorageService.SaveFileAsync(imageFile);
                        
                        var movieImage = new MovieImage
                        {
                            MovieId = movie.Id,
                            Path = filePath,
                            Order = order++
                        };
                        
                        await _unitOfWork.Movies.GetDbContext().MovieImages.AddAsync(movieImage);
                    }
                }
                await _unitOfWork.SaveChangesAsync();
            }

            if (viewModel.SelectedActorIds != null && viewModel.SelectedActorIds.Any())
            {
                foreach (var actorId in viewModel.SelectedActorIds)
                {
                    var movieActor = new MovieActor
                    {
                        MovieId = movie.Id,
                        ActorId = actorId
                    };
                    
                    await _unitOfWork.Movies.GetDbContext().MovieActors.AddAsync(movieActor);
                }
                await _unitOfWork.SaveChangesAsync();
            }

            return movie;
        }

        public async Task<Movie> UpdateMovieAsync(int id, MovieCreateEditVM viewModel)
        {
            Expression<Func<Movie, object>>[] includes = { m => m.Actors };
            var movie = await _unitOfWork.Movies.GetFirstOrDefaultAsync(
                m => m.Id == id, 
                includes: includes);
            
            if (movie == null)
                return null;

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

            if (viewModel.PosterFile != null && viewModel.PosterFile.Length > 0)
            {
                string fileName = await _fileStorageService.SaveFileAsync(viewModel.PosterFile);
                movie.PosterUrl = fileName;
            }
            else if (!string.IsNullOrEmpty(viewModel.PosterUrl) && viewModel.PosterUrl != movie.PosterUrl)
            {
                movie.PosterUrl = viewModel.PosterUrl;
            }

            if (viewModel.MovieImageFiles != null && viewModel.MovieImageFiles.Any(f => f != null && f.Length > 0))
            {
                var dbContext = _unitOfWork.Movies.GetDbContext();
                var highestOrder = await dbContext.MovieImages
                    .Where(mi => mi.MovieId == id)
                    .Select(mi => mi.Order)
                    .DefaultIfEmpty(0)
                    .MaxAsync();

                int order = highestOrder + 1;
                
                foreach (var imageFile in viewModel.MovieImageFiles)
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        string filePath = await _fileStorageService.SaveFileAsync(imageFile);
                        
                        var movieImage = new MovieImage
                        {
                            MovieId = movie.Id,
                            Path = filePath,
                            Order = order++
                        };
                        
                        await dbContext.MovieImages.AddAsync(movieImage);
                    }
                }
            }

            if (movie.Actors != null)
            {
                var dbContext = _unitOfWork.Movies.GetDbContext();
                dbContext.MovieActors.RemoveRange(movie.Actors);
            }
            
            if (viewModel.SelectedActorIds != null && viewModel.SelectedActorIds.Any())
            {
                var dbContext = _unitOfWork.Movies.GetDbContext();
                foreach (var actorId in viewModel.SelectedActorIds)
                {
                    await dbContext.MovieActors.AddAsync(new MovieActor
                    {
                        MovieId = movie.Id,
                        ActorId = actorId
                    });
                }
            }
            
            _unitOfWork.Movies.Update(movie);
            await _unitOfWork.SaveChangesAsync();
            
            return movie;
        }

        public async Task DeleteMovieAsync(int id)
        {
            Expression<Func<Movie, object>>[] includes = { m => m.Images, m => m.Actors };
            var movie = await _unitOfWork.Movies.GetFirstOrDefaultAsync(
                m => m.Id == id,
                includes: includes);
                
            if (movie != null)
            {
                var dbContext = _unitOfWork.Movies.GetDbContext();
                
                if (movie.Actors != null && movie.Actors.Any())
                {
                    dbContext.MovieActors.RemoveRange(movie.Actors);
                }
                
                if (movie.Images != null && movie.Images.Any())
                {
                    dbContext.MovieImages.RemoveRange(movie.Images);
                }
                
                _unitOfWork.Movies.Remove(movie);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<int> GetNowShowingCountAsync()
        {
            var movies = await _unitOfWork.Movies.GetAllAsync();
            return movies.Count(m => m.Status == MovieStatus.NowShowing);
        }

        public async Task<int> GetComingSoonCountAsync()
        {
            var movies = await _unitOfWork.Movies.GetAllAsync();
            return movies.Count(m => m.Status == MovieStatus.ComingSoon);
        }

        public async Task DeleteImageAsync(int imageId)
        {
            var dbContext = _unitOfWork.Movies.GetDbContext();
            var image = await dbContext.MovieImages.FindAsync(imageId);
            if (image == null)
                return;

            // Delete the file
            await _fileStorageService.DeleteFileAsync(image.Path);
            
            // Remove from database
            dbContext.MovieImages.Remove(image);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateImageOrderAsync(int imageId, int newOrder)
        {
            var dbContext = _unitOfWork.Movies.GetDbContext();
            var image = await dbContext.MovieImages.FindAsync(imageId);
            if (image == null)
                return;

            var movieImagesQuery = dbContext.MovieImages
                .Where(mi => mi.MovieId == image.MovieId)
                .OrderBy(mi => mi.Order);
            
            var movieImages = await movieImagesQuery.ToListAsync();
            
            if (newOrder < 1)
            {
                newOrder = 1;
            }
            else if (newOrder > movieImages.Count)
            {
                newOrder = movieImages.Count;
            }
            
            int oldOrder = image.Order;
            
            if (newOrder < oldOrder)
            {
                foreach (var img in movieImages.Where(mi => mi.Order >= newOrder && mi.Order < oldOrder))
                {
                    img.Order++;
                }
            }
            else if (newOrder > oldOrder)
            {
                foreach (var img in movieImages.Where(mi => mi.Order <= newOrder && mi.Order > oldOrder))
                {
                    img.Order--;
                }
            }
            
            image.Order = newOrder;
            
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> MovieExistsAsync(int id)
        {
            return await _unitOfWork.Movies.AnyAsync(e => e.Id == id);
        }

        private List<Movie> ApplySorting(List<Movie> movies, string sortField, string sortOrder)
        {
            return sortField.ToLower() switch
            {
                "title" => sortOrder.ToLower() == "desc" 
                    ? movies.OrderByDescending(m => m.Title).ToList() 
                    : movies.OrderBy(m => m.Title).ToList(),
                "category" => sortOrder.ToLower() == "desc" 
                    ? movies.OrderByDescending(m => m.Category != null ? m.Category.Name : "Uncategorized").ToList() 
                    : movies.OrderBy(m => m.Category != null ? m.Category.Name : "Uncategorized").ToList(),
                "year" => sortOrder.ToLower() == "desc" 
                    ? movies.OrderByDescending(m => m.ReleaseYear).ToList() 
                    : movies.OrderBy(m => m.ReleaseYear).ToList(),
                "status" => sortOrder.ToLower() == "desc" 
                    ? movies.OrderByDescending(m => m.Status).ToList() 
                    : movies.OrderBy(m => m.Status).ToList(),
                _ => sortOrder.ToLower() == "desc" 
                    ? movies.OrderByDescending(m => m.Id).ToList() 
                    : movies.OrderBy(m => m.Id).ToList()
            };
        }
    }
}