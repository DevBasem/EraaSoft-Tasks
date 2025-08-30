using System.ComponentModel.DataAnnotations;

namespace MovieApp.Models;

public class Category
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public ICollection<Movie> Movies { get; set; } = new List<Movie>();
}