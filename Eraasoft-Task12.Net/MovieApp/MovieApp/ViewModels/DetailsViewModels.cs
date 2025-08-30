using MovieApp.Models;
using System.Collections.Generic;

namespace MovieApp.ViewModels
{
    public class CinemaDetailsVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string LogoPath { get; set; }
        public List<ActorMovieVM> Movies { get; set; } = new List<ActorMovieVM>();
    }

    public class CinemaVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string City { get; set; }
        public int MovieCount { get; set; }
    }
}