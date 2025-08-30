using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieApp.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Cinemas",
                columns: new[] { "Id", "Name", "Location", "LogoPath", "Address", "City", "PhoneNumber", "Email" },
                values: new object[,]
                {
                    { 1, "Downtown Cinema", "Downtown", "/images/cinemas/downtown-logo.png", "123 Main St", "Metropolis", "555-0100", "contact@downtowncinema.test" },
                    { 2, "Grand Mall Cinema", "Grand Mall", "/images/cinemas/grandmall-logo.png", "500 Mall Ave", "Metropolis", "555-0101", "contact@grandmallcinema.test" }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name", "Description" },
                values: new object[,]
                {
                    { 1, "Action", "High energy, stunts and excitement" },
                    { 2, "Drama", "Character-driven stories" },
                    { 3, "Comedy", "Humor and light-hearted themes" }
                });

            migrationBuilder.InsertData(
                table: "Actors",
                columns: new[] { "Id", "FullName", "Bio", "ProfileImagePath" },
                values: new object[,]
                {
                    { 1, "Cast Member 1", null, "/images/cast/cast1.png" },
                    { 2, "Cast Member 2", null, "/images/cast/cast2.png" },
                    { 3, "Cast Member 3", null, "/images/cast/cast3.png" },
                    { 4, "Cast Member 4", null, "/images/cast/cast4.png" },
                    { 5, "Cast Member 5", null, "/images/cast/cast5.png" },
                    { 6, "Cast Member 6", null, "/images/cast/cast6.png" }
                });

            migrationBuilder.InsertData(
                table: "Movies",
                columns: new[] { "Id", "Title", "Description", "ReleaseYear", "DurationMinutes", "Status", "PosterUrl", "Price", "TotalTickets", "ReservedTickets", "CinemaId", "CategoryId" },
                values: new object[,]
                {
                    { 1,  "Movie 1",  "Seeded movie 1",  2021, 110, 2, "/images/movies/movie1.png",  10.00m, 120, 20, 1, 1 },
                    { 2,  "Movie 2",  "Seeded movie 2",  2021, 115, 2, "/images/movies/movie2.png",  10.50m, 120, 30, 1, 2 },
                    { 3,  "Movie 3",  "Seeded movie 3",  2022, 100, 2, "/images/movies/movie3.png",  11.00m, 120, 25, 2, 3 },
                    { 4,  "Movie 4",  "Seeded movie 4",  2022, 95,  1, "/images/movies/movie4.png",  9.50m,  100, 10, 2, 1 },
                    { 5,  "Movie 5",  "Seeded movie 5",  2023, 105, 1, "/images/movies/movie5.png",  12.00m, 150, 40, 1, 2 },
                    { 6,  "Movie 6",  "Seeded movie 6",  2023, 98,  1, "/images/movies/movie6.png",  8.99m,  90,  5,  1, 3 },
                    { 7,  "Movie 7",  "Seeded movie 7",  2024, 130, 2, "/images/movies/movie7.png",  13.50m, 160, 70, 2, 1 },
                    { 8,  "Movie 8",  "Seeded movie 8",  2024, 102, 2, "/images/movies/movie8.png",  9.99m,  110, 15, 2, 2 },
                    { 9,  "Movie 9",  "Seeded movie 9",  2024, 125, 2, "/images/movies/movie9.png",  14.00m, 170, 60, 1, 3 },
                    { 10, "Movie 10", "Seeded movie 10", 2025, 112, 1, "/images/movies/movie10.png", 10.99m, 120, 12, 2, 1 }
                });

            migrationBuilder.InsertData(
                table: "MovieActors",
                columns: new[] { "MovieId", "ActorId" },
                values: new object[,]
                {
                    { 1, 1 }, { 1, 2 },
                    { 2, 2 }, { 2, 3 },
                    { 3, 3 }, { 3, 4 },
                    { 4, 4 }, { 4, 5 },
                    { 5, 5 }, { 5, 6 },
                    { 6, 6 }, { 6, 1 },
                    { 7, 1 }, { 7, 3 },
                    { 8, 2 }, { 8, 4 },
                    { 9, 3 }, { 9, 5 },
                    { 10, 4 }, { 10, 6 }
                });

            migrationBuilder.InsertData(
                table: "MovieImages",
                columns: new[] { "Id", "MovieId", "Path", "Order" },
                values: new object[,]
                {
                    { 1, 1,  "/images/movies/movie1.png",  0 },
                    { 2, 2,  "/images/movies/movie2.png",  0 },
                    { 3, 3,  "/images/movies/movie3.png",  0 },
                    { 4, 4,  "/images/movies/movie4.png",  0 },
                    { 5, 5,  "/images/movies/movie5.png",  0 },
                    { 6, 6,  "/images/movies/movie6.png",  0 },
                    { 7, 7,  "/images/movies/movie7.png",  0 },
                    { 8, 8,  "/images/movies/movie8.png",  0 },
                    { 9, 9,  "/images/movies/movie9.png",  0 },
                    { 10, 10, "/images/movies/movie10.png", 0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MovieActors",
                keyColumns: new[] { "MovieId", "ActorId" },
                keyValues: new object[,]
                {
                    { 1, 1 }, { 1, 2 },
                    { 2, 2 }, { 2, 3 },
                    { 3, 3 }, { 3, 4 },
                    { 4, 4 }, { 4, 5 },
                    { 5, 5 }, { 5, 6 },
                    { 6, 6 }, { 6, 1 },
                    { 7, 1 }, { 7, 3 },
                    { 8, 2 }, { 8, 4 },
                    { 9, 3 }, { 9, 5 },
                    { 10, 4 }, { 10, 6 }
                });

            migrationBuilder.DeleteData(
                table: "MovieImages",
                keyColumn: "Id",
                keyValues: new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });

            // Then principals
            migrationBuilder.DeleteData(
                table: "Movies",
                keyColumn: "Id",
                keyValues: new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });

            migrationBuilder.DeleteData(
                table: "Actors",
                keyColumn: "Id",
                keyValues: new object[] { 1, 2, 3, 4, 5, 6 });

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValues: new object[] { 1, 2, 3 });

            migrationBuilder.DeleteData(
                table: "Cinemas",
                keyColumn: "Id",
                keyValues: new object[] { 1, 2 });
        }
    }
}
