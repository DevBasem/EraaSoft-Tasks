namespace MovieApp.Models;

public class Cinema
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Location { get; set; }

    public string? LogoPath { get; set; }

    public string? Address { get; set; }

    public string? City { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public ICollection<Movie> Movies { get; set; } = new List<Movie>();
}