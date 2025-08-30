using Microsoft.AspNetCore.Http;
using MovieApp.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MovieApp.ViewModels.Admin
{
    public class DashboardVM
    {
        public int TotalMovies { get; set; }
        public int TotalActors { get; set; }
        public int TotalCategories { get; set; }
        public int TotalCinemas { get; set; }
        public int NowShowingMovies { get; set; }
        public int ComingSoonMovies { get; set; }
        public List<MovieItemVM> RecentMovies { get; set; } = new List<MovieItemVM>();
    }

    public class MovieItemVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string CategoryName { get; set; }
        public string? PosterUrl { get; set; }
        public int? ReleaseYear { get; set; }
        public MovieStatus Status { get; set; }
    }

    public class MovieDetailsVM
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? ReleaseYear { get; set; }
        public int? DurationMinutes { get; set; }
        public string? PosterUrl { get; set; }
        public string? TrailerUrl { get; set; }
        public decimal Price { get; set; }
        public MovieStatus Status { get; set; }
        public int TotalTickets { get; set; }
        public int ReservedTickets { get; set; }
        public int AvailableTickets { get; set; }
        
        public string? CategoryName { get; set; }
        public int? CategoryId { get; set; }
        
        public string? CinemaName { get; set; }
        public int? CinemaId { get; set; }
        public string? CinemaLocation { get; set; }
        public string? CinemaCity { get; set; }
        public string? CinemaAddress { get; set; }
        public string? CinemaPhone { get; set; }
        public string? CinemaEmail { get; set; }
        
        public List<MovieImage>? Images { get; set; }
        public List<ActorVM>? Actors { get; set; }
    }

    public class MovieImageVM
    {
        public int Id { get; set; }
        public string Path { get; set; } = string.Empty;
        public int Order { get; set; }
        public int MovieId { get; set; }
    }

    public class MovieCreateEditVM
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Movie title is required")]
        public string Title { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        public int? ReleaseYear { get; set; }
        
        public int? DurationMinutes { get; set; }
        
        public MovieStatus Status { get; set; } = MovieStatus.ComingSoon;
        
        // Explicitly making PosterUrl nullable and optional
        public string? PosterUrl { get; set; }
        
        // File upload properties
        public IFormFile? PosterFile { get; set; }
        public List<IFormFile> MovieImageFiles { get; set; } = new List<IFormFile>();
        
        // Existing movie images for the edit view
        public List<MovieImage> MovieImages { get; set; } = new List<MovieImage>();
        
        // Explicitly making TrailerUrl nullable and optional
        public string? TrailerUrl { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value")]
        public decimal Price { get; set; }
        
        [Range(0, int.MaxValue, ErrorMessage = "Total tickets must be a positive value")]
        public int TotalTickets { get; set; }
        
        [Range(0, int.MaxValue, ErrorMessage = "Reserved tickets must be a positive value")]
        [ReservedTicketsValidation(ErrorMessage = "Reserved tickets cannot exceed total tickets")]
        public int ReservedTickets { get; set; }
        
        public int? CategoryId { get; set; }
        
        public int? CinemaId { get; set; }
        
        public List<CategoryVM> Categories { get; set; } = new List<CategoryVM>();
        
        public List<CinemaVM> Cinemas { get; set; } = new List<CinemaVM>();
        
        public List<ActorSelectVM> AllActors { get; set; } = new List<ActorSelectVM>();
        
        public List<int> SelectedActorIds { get; set; } = new List<int>();
    }

    public class ReservedTicketsValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (MovieCreateEditVM)validationContext.ObjectInstance;
            
            if (model.ReservedTickets > model.TotalTickets)
            {
                return new ValidationResult(ErrorMessage ?? "Reserved tickets cannot exceed total tickets");
            }
            
            return ValidationResult.Success;
        }
    }

    public class ActorSelectVM
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }

    public class CategoryCreateEditVM
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Category name is required")]
        public string Name { get; set; } = string.Empty;
        
        public string? Description { get; set; }
    }

    public class ActorCreateEditVM
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Actor name is required")]
        public string FullName { get; set; } = string.Empty;
        
        public string? Bio { get; set; }
        
        // Make this property nullable and optional
        public string? ProfileImagePath { get; set; }
        
        // New property for file upload
        public IFormFile? ProfileImageFile { get; set; }
    }

    public class CinemaCreateEditVM
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Cinema name is required")]
        public string Name { get; set; } = string.Empty;
        
        public string? Location { get; set; }
        
        public string? Address { get; set; }
        
        public string? City { get; set; }
        
        public string? PhoneNumber { get; set; }
        
        public string? Email { get; set; }
        
        // Make this property nullable and optional
        public string? LogoPath { get; set; }
        
        // New property for file upload
        public IFormFile? LogoFile { get; set; }
    }
    
    public class CategoryVM
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public int MovieCount { get; set; }
        public List<MovieItemVM> Movies { get; set; } = new List<MovieItemVM>();
    }
    
    public class CinemaVM
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Location { get; set; }
        public string? City { get; set; }
        public string? LogoPath { get; set; }
        public int MovieCount { get; set; }
    }

    public class ActorVM
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? ProfileImagePath { get; set; }
        public int MovieCount { get; set; }
    }
}