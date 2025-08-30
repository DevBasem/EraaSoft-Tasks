using System.ComponentModel.DataAnnotations;

namespace MovieApp.Models;

public class Actor
{
    public int Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string? Bio { get; set; }

    public string? ProfileImagePath { get; set; }

    public ICollection<MovieActor> Movies { get; set; } = new List<MovieActor>();
}