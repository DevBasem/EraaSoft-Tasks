namespace MovieApp.ViewModels
{
    public class CategoryVM
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int MovieCount { get; set; }
        public int NowShowingCount { get; set; }
        public int ComingSoonCount { get; set; }
    }

    public class CategoryDetailsVM
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<MovieVM> Movies { get; set; } = new List<MovieVM>();
        public int TotalMovies { get; set; }
        public int NowShowingMovies { get; set; }
        public int ComingSoonMovies { get; set; }
    }

    public class CategoriesIndexVM
    {
        public List<CategoryVM> Categories { get; set; } = new List<CategoryVM>();
        public int TotalCategories { get; set; }
        public int TotalMovies { get; set; }
        public string MostPopularCategory { get; set; } = string.Empty;
    }
}