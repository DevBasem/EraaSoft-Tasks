using MovieApp.Models;

namespace MovieApp.ViewModels
{
    public class ActorVM
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? ProfileImagePath { get; set; }
        public int MovieCount { get; set; }
        public List<string> RecentMovies { get; set; } = new List<string>();
    }

    public class ActorDetailsVM
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? ProfileImagePath { get; set; }
        public List<ActorMovieVM> Movies { get; set; } = new List<ActorMovieVM>();
        public int TotalMovies { get; set; }
        public int NowShowingMovies { get; set; }
        public int ComingSoonMovies { get; set; }
    }

    public class ActorMovieVM
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int? ReleaseYear { get; set; }
        public MovieStatus Status { get; set; }
        public string? PosterUrl { get; set; }
        public decimal Price { get; set; }
        public int? DurationMinutes { get; set; }
        public string? CategoryName { get; set; }
        public string? CinemaName { get; set; }
    }

    public class ActorsIndexVM
    {
        public List<ActorVM> Actors { get; set; } = new List<ActorVM>();
        public int TotalActors { get; set; }
        public int TotalMovieAppearances { get; set; }
    }
}