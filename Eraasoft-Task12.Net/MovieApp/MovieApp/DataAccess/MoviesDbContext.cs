using Microsoft.EntityFrameworkCore;
using MovieApp.Models;

namespace MovieApp.DataAccess
{
    public class MoviesDbContext : DbContext
    {
        public DbSet<Movie> Movies => Set<Movie>();
        public DbSet<Actor> Actors => Set<Actor>();
        public DbSet<Cinema> Cinemas => Set<Cinema>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<MovieActor> MovieActors => Set<MovieActor>();
        public DbSet<MovieImage> MovieImages => Set<MovieImage>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=MovieApp;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MovieActor>(entity =>
            {
                entity.HasKey(ma => new { ma.MovieId, ma.ActorId });

                entity.HasOne(ma => ma.Movie)
                      .WithMany(m => m.Actors)
                      .HasForeignKey(ma => ma.MovieId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ma => ma.Actor)
                      .WithMany(a => a.Movies)
                      .HasForeignKey(ma => ma.ActorId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<MovieImage>(entity =>
            {
                entity.Property(mi => mi.Path)
                      .IsRequired()
                      .HasMaxLength(500);

                entity.Property(mi => mi.Order).HasDefaultValue(0);

                entity.HasOne(mi => mi.Movie)
                      .WithMany(m => m.Images)
                      .HasForeignKey(mi => mi.MovieId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Movie>(entity =>
            {
                entity.Property(m => m.Title)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(m => m.Description)
                      .HasMaxLength(4000);

                entity.Property(m => m.PosterUrl)
                      .HasMaxLength(500);

                entity.Property(m => m.Price)
                      .HasColumnType("decimal(10,2)");

                entity.ToTable(tb =>
                {
                    tb.HasCheckConstraint("CK_Movie_ReleaseYear", "[ReleaseYear] IS NULL OR ([ReleaseYear] >= 1888 AND [ReleaseYear] <= 3000)");
                    tb.HasCheckConstraint("CK_Movie_DurationMinutes", "[DurationMinutes] IS NULL OR ([DurationMinutes] >= 0 AND [DurationMinutes] <= 1000)");
                    tb.HasCheckConstraint("CK_Movie_Tickets_Total", "[TotalTickets] >= 0");
                    tb.HasCheckConstraint("CK_Movie_Tickets_Reserved", "[ReservedTickets] >= 0 AND [ReservedTickets] <= [TotalTickets]");
                });

                entity.HasOne(m => m.Cinema)
                      .WithMany(c => c.Movies)
                      .HasForeignKey(m => m.CinemaId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(m => m.Category)
                      .WithMany(c => c.Movies)
                      .HasForeignKey(m => m.CategoryId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(c => c.Name)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(c => c.Description)
                      .HasMaxLength(300);
            });

            modelBuilder.Entity<Actor>(entity =>
            {
                entity.Property(a => a.FullName)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(a => a.ProfileImagePath)
                      .HasMaxLength(500);

                entity.Property(a => a.Bio)
                      .HasMaxLength(2000);
            });

            modelBuilder.Entity<Cinema>(entity =>
            {
                entity.Property(c => c.Name)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.Property(c => c.Location)
                      .HasMaxLength(250);

                entity.Property(c => c.LogoPath)
                      .HasMaxLength(500);

                entity.Property(c => c.Address)
                      .HasMaxLength(300);

                entity.Property(c => c.City)
                      .HasMaxLength(100);

                entity.Property(c => c.PhoneNumber)
                      .HasMaxLength(50);

                entity.Property(c => c.Email)
                      .HasMaxLength(150);
            });
        }
    }
}
