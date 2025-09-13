using Microsoft.AspNetCore.Http;
using MovieApp.Models;
using MovieApp.Repositories.IRepositories;
using MovieApp.Services.Interfaces;
using MovieApp.ViewModels;
using MovieApp.ViewModels.Admin;
using System.Linq.Expressions;

namespace MovieApp.Services
{
    public class ActorService : IActorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorageService _fileStorageService;

        public ActorService(IUnitOfWork unitOfWork, IFileStorageService fileStorageService)
        {
            _unitOfWork = unitOfWork;
            _fileStorageService = fileStorageService;
        }

        public async Task<List<ViewModels.Admin.ActorVM>> GetAllActorsAsync()
        {
            var actors = await _unitOfWork.Actors.GetAllAsync(
                includes: new Expression<Func<Actor, object>>[] { a => a.Movies }
            );

            return actors
                .Select(a => new ViewModels.Admin.ActorVM
                {
                    Id = a.Id,
                    FullName = a.FullName,
                    ProfileImagePath = a.ProfileImagePath,
                    Bio = a.Bio,
                    MovieCount = a.Movies.Count
                })
                .OrderBy(a => a.FullName)
                .ToList();
        }

        public async Task<Actor> GetActorByIdAsync(int id)
        {
            return await _unitOfWork.Actors.GetByIdAsync(id);
        }

        public async Task<Actor> GetActorWithMoviesAsync(int id)
        {
            return await _unitOfWork.Actors.GetActorWithMoviesAsync(id);
        }

        public async Task<ViewModels.Admin.ActorVM> GetActorDetailsAsync(int id)
        {
            var actor = await _unitOfWork.Actors.GetActorWithMoviesAsync(id);

            if (actor == null)
                return null;

            return new ViewModels.Admin.ActorVM
            {
                Id = actor.Id,
                FullName = actor.FullName,
                ProfileImagePath = actor.ProfileImagePath,
                Bio = actor.Bio,
                MovieCount = actor.Movies.Count
            };
        }

        public async Task<ActorsIndexVM> GetActorsForPublicIndexAsync()
        {
            var actors = await _unitOfWork.Actors.GetAllAsync(
                includes: new Expression<Func<Actor, object>>[] { a => a.Movies }
            );

            return new ActorsIndexVM
            {
                Actors = actors.Select(a => new ViewModels.ActorVM
                {
                    Id = a.Id,
                    FullName = a.FullName,
                    Bio = a.Bio,
                    ProfileImagePath = !string.IsNullOrEmpty(a.ProfileImagePath) ? a.ProfileImagePath : "/images/default-actor.svg",
                    MovieCount = a.Movies.Count(),
                    RecentMovies = a.Movies
                        .Where(ma => ma != null && ma.Movie != null)
                        .Take(3)
                        .Select(ma => ma.Movie.Title)
                        .ToList()
                }).ToList(),
                TotalActors = actors.Count,
                TotalMovieAppearances = actors.Sum(a => a.Movies.Count())
            };
        }

        public async Task<ActorDetailsVM> GetActorForPublicDetailsAsync(int id)
        {
            var actor = await _unitOfWork.Actors.GetActorWithMoviesAsync(id);

            if (actor == null)
                return null;

            return new ActorDetailsVM
            {
                Id = actor.Id,
                FullName = actor.FullName,
                Bio = actor.Bio,
                ProfileImagePath = !string.IsNullOrEmpty(actor.ProfileImagePath) ? actor.ProfileImagePath : "/images/default-actor.svg",
                Movies = actor.Movies
                    .Where(ma => ma != null && ma.Movie != null)
                    .Select(ma => new ActorMovieVM
                    {
                        Id = ma.Movie.Id,
                        Title = ma.Movie.Title,
                        ReleaseYear = ma.Movie.ReleaseYear,
                        Status = ma.Movie.Status,
                        PosterUrl = ma.Movie.PosterUrl,
                        Price = ma.Movie.Price,
                        DurationMinutes = ma.Movie.DurationMinutes,
                        CategoryName = ma.Movie.Category?.Name,
                        CinemaName = ma.Movie.Cinema?.Name
                    })
                    .OrderByDescending(m => m.ReleaseYear)
                    .ToList(),
                TotalMovies = actor.Movies.Count(ma => ma != null && ma.Movie != null),
                NowShowingMovies = actor.Movies.Count(ma => ma != null && ma.Movie != null && ma.Movie.Status == MovieStatus.NowShowing),
                ComingSoonMovies = actor.Movies.Count(ma => ma != null && ma.Movie != null && ma.Movie.Status == MovieStatus.ComingSoon)
            };
        }

        public async Task<Actor> CreateActorAsync(ActorCreateEditVM viewModel)
        {
            var actor = new Actor
            {
                FullName = viewModel.FullName,
                Bio = viewModel.Bio,
                ProfileImagePath = viewModel.ProfileImagePath
            };

            await _unitOfWork.Actors.AddAsync(actor);
            await _unitOfWork.SaveChangesAsync();

            if (viewModel.ProfileImageFile != null && viewModel.ProfileImageFile.Length > 0)
            {
                string fileName = await _fileStorageService.SaveFileAsync(viewModel.ProfileImageFile, "uploads/actors");
                actor.ProfileImagePath = fileName;
                _unitOfWork.Actors.Update(actor);
                await _unitOfWork.SaveChangesAsync();
            }

            return actor;
        }

        public async Task<Actor> UpdateActorAsync(int id, ActorCreateEditVM viewModel)
        {
            var actor = await _unitOfWork.Actors.GetByIdAsync(id);

            if (actor == null)
                return null;

            actor.FullName = viewModel.FullName;
            actor.Bio = viewModel.Bio;
            
            if (viewModel.ProfileImageFile != null && viewModel.ProfileImageFile.Length > 0)
            {
                string fileName = await _fileStorageService.SaveFileAsync(viewModel.ProfileImageFile, "uploads/actors");
                actor.ProfileImagePath = fileName;
            }
            else if (!string.IsNullOrEmpty(viewModel.ProfileImagePath) && viewModel.ProfileImagePath != actor.ProfileImagePath)
            {
                actor.ProfileImagePath = viewModel.ProfileImagePath;
            }

            _unitOfWork.Actors.Update(actor);
            await _unitOfWork.SaveChangesAsync();
            
            return actor;
        }

        public async Task DeleteActorAsync(int id)
        {
            var actor = await _unitOfWork.Actors.GetByIdAsync(id);
            
            if (actor != null)
            {
                if (!string.IsNullOrEmpty(actor.ProfileImagePath))
                {
                    await _fileStorageService.DeleteFileAsync(actor.ProfileImagePath);
                }
                
                _unitOfWork.Actors.Remove(actor);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<bool> ActorExistsAsync(int id)
        {
            return await _unitOfWork.Actors.AnyAsync(e => e.Id == id);
        }
    }
}