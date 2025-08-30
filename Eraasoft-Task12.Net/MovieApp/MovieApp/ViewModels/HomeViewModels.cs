using MovieApp.Models;

namespace MovieApp.ViewModels
{
    public class HomeIndexVM
    {
        public List<MovieVM> FeaturedMovies { get; set; } = new List<MovieVM>();
        public int TotalMovies { get; set; }
        public int TotalActors { get; set; }
        public int TotalCategories { get; set; }
        public int TotalCinemas { get; set; }
    }
}