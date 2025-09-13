using MovieApp.Models;
using MovieApp.Repositories.IRepositories;
using MovieApp.Services.Interfaces;
using MovieApp.ViewModels.Admin;
using System.Linq.Expressions;

namespace MovieApp.Services
{
    public class CinemaService : ICinemaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorageService _fileStorageService;

        public CinemaService(IUnitOfWork unitOfWork, IFileStorageService fileStorageService)
        {
            _unitOfWork = unitOfWork;
            _fileStorageService = fileStorageService;
        }

        public async Task<List<CinemaVM>> GetAllCinemasAsync()
        {
            var cinemas = await _unitOfWork.Cinemas.GetCinemasWithMoviesCountAsync();

            return cinemas.Select(c => new CinemaVM
            {
                Id = c.Id,
                Name = c.Name,
                Location = c.Location,
                City = c.City,
                LogoPath = c.LogoPath,
                MovieCount = c.Movies?.Count() ?? 0
            })
            .OrderBy(c => c.Name)
            .ToList();
        }

        public async Task<Cinema> GetCinemaByIdAsync(int id)
        {
            return await _unitOfWork.Cinemas.GetByIdAsync(id);
        }

        public async Task<CinemaVM> GetCinemaDetailsAsync(int id)
        {
            Expression<Func<Cinema, object>>[] includes = { c => c.Movies };
            var cinema = await _unitOfWork.Cinemas.GetFirstOrDefaultAsync(
                c => c.Id == id,
                includes: includes
            );

            if (cinema == null)
                return null;

            var movieIds = cinema.Movies.Select(m => m.Id).ToList();
            Expression<Func<Movie, object>>[] movieIncludes = { m => m.Category };
            await _unitOfWork.Movies.GetAllAsync(
                filter: m => movieIds.Contains(m.Id),
                includes: movieIncludes
            );

            return new CinemaVM
            {
                Id = cinema.Id,
                Name = cinema.Name,
                Location = cinema.Location,
                City = cinema.City,
                LogoPath = cinema.LogoPath,
                MovieCount = cinema.Movies.Count()
            };
        }

        public async Task<Cinema> CreateCinemaAsync(CinemaCreateEditVM viewModel)
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

            await _unitOfWork.Cinemas.AddAsync(cinema);
            await _unitOfWork.SaveChangesAsync();
            
            if (viewModel.LogoFile != null && viewModel.LogoFile.Length > 0)
            {
                string fileName = await _fileStorageService.SaveFileAsync(viewModel.LogoFile, "uploads/cinemas");
                cinema.LogoPath = fileName;
                _unitOfWork.Cinemas.Update(cinema);
                await _unitOfWork.SaveChangesAsync();
            }
            
            return cinema;
        }

        public async Task<Cinema> UpdateCinemaAsync(int id, CinemaCreateEditVM viewModel)
        {
            var cinema = await _unitOfWork.Cinemas.GetByIdAsync(id);

            if (cinema == null)
                return null;

            cinema.Name = viewModel.Name;
            cinema.Location = viewModel.Location;
            cinema.Address = viewModel.Address;
            cinema.City = viewModel.City;
            cinema.PhoneNumber = viewModel.PhoneNumber;
            cinema.Email = viewModel.Email;
            
            if (viewModel.LogoFile != null && viewModel.LogoFile.Length > 0)
            {
                // Delete old logo if exists
                if (!string.IsNullOrEmpty(cinema.LogoPath))
                {
                    await _fileStorageService.DeleteFileAsync(cinema.LogoPath);
                }

                string fileName = await _fileStorageService.SaveFileAsync(viewModel.LogoFile, "uploads/cinemas");
                cinema.LogoPath = fileName;
            }
            else if (!string.IsNullOrEmpty(viewModel.LogoPath) && viewModel.LogoPath != cinema.LogoPath)
            {
                cinema.LogoPath = viewModel.LogoPath;
            }

            _unitOfWork.Cinemas.Update(cinema);
            await _unitOfWork.SaveChangesAsync();
            
            return cinema;
        }

        public async Task DeleteCinemaAsync(int id)
        {
            var cinema = await _unitOfWork.Cinemas.GetByIdAsync(id);
            
            if (cinema != null)
            {
                if (!string.IsNullOrEmpty(cinema.LogoPath))
                {
                    await _fileStorageService.DeleteFileAsync(cinema.LogoPath);
                }
                
                _unitOfWork.Cinemas.Remove(cinema);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<bool> CinemaExistsAsync(int id)
        {
            return await _unitOfWork.Cinemas.AnyAsync(c => c.Id == id);
        }
    }
}