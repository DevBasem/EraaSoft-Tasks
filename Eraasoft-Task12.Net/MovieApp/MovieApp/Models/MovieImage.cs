namespace MovieApp.Models;

public class MovieImage
{
    public int Id { get; set; }

    public int MovieId { get; set; }
    public Movie Movie { get; set; } = null!;

    public string Path { get; set; } = string.Empty;

    public int Order { get; set; }
}