using MovieApp.Models;

namespace MovieApp.ViewModels
{
    public class MovieVM
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? ReleaseYear { get; set; }
        public int? DurationMinutes { get; set; }
        public MovieStatus Status { get; set; }
        public string? PosterUrl { get; set; }
        public decimal Price { get; set; }
        public int TotalTickets { get; set; }
        public int ReservedTickets { get; set; }
        public int AvailableTickets { get; set; }
        public string? CategoryName { get; set; }
        public string? CinemaName { get; set; }
        public string? CinemaLocation { get; set; }
    }

    public class MovieDetailsVM
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? ReleaseYear { get; set; }
        public int? DurationMinutes { get; set; }
        public MovieStatus Status { get; set; }
        public string? PosterUrl { get; set; }
        public decimal Price { get; set; }
        public int TotalTickets { get; set; }
        public int ReservedTickets { get; set; }
        public int AvailableTickets { get; set; }
        
        // Category Info
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? CategoryDescription { get; set; }
        
        // Cinema Info
        public int? CinemaId { get; set; }
        public string? CinemaName { get; set; }
        public string? CinemaLocation { get; set; }
        public string? CinemaAddress { get; set; }
        public string? CinemaCity { get; set; }
        public string? CinemaPhone { get; set; }
        public string? CinemaEmail { get; set; }
        
        // Cast
        public List<ActorVM> Actors { get; set; } = new List<ActorVM>();
        
        // Images
        public List<MovieImageVM> Images { get; set; } = new List<MovieImageVM>();
    }

    public class MoviesIndexVM
    {
        public List<MovieVM> Movies { get; set; } = new List<MovieVM>();
        public List<CategoryVM> Categories { get; set; } = new List<CategoryVM>();
        public int TotalMovies { get; set; }
        public int NowShowingCount { get; set; }
        public int ComingSoonCount { get; set; }
        public Pager Pager { get; set; } = new Pager(0);
        public int? CurrentCategoryId { get; set; }
        public int? CurrentStatus { get; set; }
        public string? CurrentSort { get; set; }
    }

    public class MovieImageVM
    {
        public int Id { get; set; }
        public string Path { get; set; } = string.Empty;
        public int Order { get; set; }
    }
}