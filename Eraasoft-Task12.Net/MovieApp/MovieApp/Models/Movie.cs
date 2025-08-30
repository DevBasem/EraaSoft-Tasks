namespace MovieApp.Models;

public class Movie
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int? ReleaseYear { get; set; }

    public int? DurationMinutes { get; set; }

    public MovieStatus Status { get; set; } = MovieStatus.ComingSoon;

    public string? PosterUrl { get; set; }

    public string? TrailerUrl { get; set; }

    public decimal Price { get; set; }

    public int TotalTickets { get; set; }
    public int ReservedTickets { get; set; }

    public int AvailableTickets => TotalTickets - ReservedTickets;

    public int? CinemaId { get; set; }
    public Cinema? Cinema { get; set; }

    public int? CategoryId { get; set; }
    public Category? Category { get; set; }

    public ICollection<MovieActor> Actors { get; set; } = new List<MovieActor>();

    public ICollection<MovieImage> Images { get; set; } = new List<MovieImage>();
}