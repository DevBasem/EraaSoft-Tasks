using Microsoft.EntityFrameworkCore;
using P01_StudentSystem.Models;
using P01_StudentSystem.Types;

namespace P01_StudentSystem.Data
{
    public static class DatabaseSeeder
    {
        public static void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>().HasData(
                new Student
                {
                    StudentId = 1,
                    Name = "Ahmed Mohamed Ali",
                    PhoneNumber = "0123456789",
                    RegisteredOn = new DateTime(2024, 1, 15, 10, 30, 0),
                    Birthday = new DateOnly(2000, 5, 15)
                },
                new Student
                {
                    StudentId = 2,
                    Name = "Sara Hassan Ibrahim",
                    PhoneNumber = "0198765432",
                    RegisteredOn = new DateTime(2024, 1, 20, 14, 15, 0),
                    Birthday = new DateOnly(1999, 8, 22)
                },
                new Student
                {
                    StudentId = 3,
                    Name = "Omar Khaled Mahmoud",
                    PhoneNumber = null,
                    RegisteredOn = new DateTime(2024, 2, 1, 9, 0, 0),
                    Birthday = null
                },
                new Student
                {
                    StudentId = 4,
                    Name = "Fatma Adel Mostafa",
                    PhoneNumber = "0111222333",
                    RegisteredOn = new DateTime(2024, 2, 10, 16, 45, 0),
                    Birthday = new DateOnly(2001, 12, 3)
                },
                new Student
                {
                    StudentId = 5,
                    Name = "Youssef Tarek Ahmed",
                    PhoneNumber = "0155667788",
                    RegisteredOn = new DateTime(2024, 2, 15, 11, 20, 0),
                    Birthday = new DateOnly(1998, 3, 10)
                }
            );

            modelBuilder.Entity<Course>().HasData(
                new Course
                {
                    CourseId = 1,
                    Name = "C# Programming Fundamentals",
                    Description = "Learn the basics of C# programming language including syntax, OOP concepts, and basic data structures.",
                    StartDate = new DateOnly(2024, 3, 1),
                    EndDate = new DateOnly(2024, 5, 30),
                    Price = 1500.00m
                },
                new Course
                {
                    CourseId = 2,
                    Name = "ASP.NET Core Web Development",
                    Description = "Build modern web applications using ASP.NET Core, Entity Framework, and MVC architecture.",
                    StartDate = new DateOnly(2024, 4, 1),
                    EndDate = new DateOnly(2024, 7, 31),
                    Price = 2500.00m
                },
                new Course
                {
                    CourseId = 3,
                    Name = "Database Design and SQL",
                    Description = "Master database design principles, SQL queries, and database optimization techniques.",
                    StartDate = new DateOnly(2024, 3, 15),
                    EndDate = new DateOnly(2024, 6, 15),
                    Price = 1800.00m
                },
                new Course
                {
                    CourseId = 4,
                    Name = "JavaScript for Beginners",
                    Description = null,
                    StartDate = new DateOnly(2024, 5, 1),
                    EndDate = new DateOnly(2024, 7, 1),
                    Price = 1200.00m
                },
                new Course
                {
                    CourseId = 5,
                    Name = "React Frontend Development",
                    Description = "Build interactive user interfaces using React, hooks, and modern JavaScript features.",
                    StartDate = new DateOnly(2024, 6, 1),
                    EndDate = new DateOnly(2024, 8, 31),
                    Price = 2200.00m
                }
            );

            modelBuilder.Entity<Resource>().HasData(
                new
                {
                    ResourceId = 1,
                    Name = "C# Basics Video Tutorial",
                    Url = "https://example.com/csharp-basics-video",
                    ResourceType = ResourceType.Video,
                    CourseId = 1
                },
                new
                {
                    ResourceId = 2,
                    Name = "C# Reference Guide",
                    Url = "https://example.com/csharp-reference.pdf",
                    ResourceType = ResourceType.Document,
                    CourseId = 1
                },
                new
                {
                    ResourceId = 3,
                    Name = "OOP Concepts Presentation",
                    Url = "https://example.com/oop-presentation.pptx",
                    ResourceType = ResourceType.Presentation,
                    CourseId = 1
                },
                new
                {
                    ResourceId = 4,
                    Name = "ASP.NET Core Setup Guide",
                    Url = "https://example.com/aspnet-setup.pdf",
                    ResourceType = ResourceType.Document,
                    CourseId = 2
                },
                new
                {
                    ResourceId = 5,
                    Name = "MVC Architecture Video",
                    Url = "https://example.com/mvc-architecture-video",
                    ResourceType = ResourceType.Video,
                    CourseId = 2
                },
                new
                {
                    ResourceId = 6,
                    Name = "SQL Query Examples",
                    Url = "https://example.com/sql-examples",
                    ResourceType = ResourceType.Other,
                    CourseId = 3
                },
                new
                {
                    ResourceId = 7,
                    Name = "Database Design Principles",
                    Url = "https://example.com/db-design.pptx",
                    ResourceType = ResourceType.Presentation,
                    CourseId = 3
                },
                new
                {
                    ResourceId = 8,
                    Name = "JavaScript Fundamentals Video",
                    Url = "https://example.com/js-fundamentals",
                    ResourceType = ResourceType.Video,
                    CourseId = 4
                },
                new
                {
                    ResourceId = 9,
                    Name = "React Components Guide",
                    Url = "https://example.com/react-components.pdf",
                    ResourceType = ResourceType.Document,
                    CourseId = 5
                },
                new
                {
                    ResourceId = 10,
                    Name = "React Hooks Tutorial",
                    Url = "https://example.com/react-hooks-video",
                    ResourceType = ResourceType.Video,
                    CourseId = 5
                }
            );

            modelBuilder.Entity<StudentCourse>().HasData(
                new { StudentId = 1, CourseId = 1 },
                new { StudentId = 1, CourseId = 3 },
                new { StudentId = 2, CourseId = 2 },
                new { StudentId = 2, CourseId = 3 },
                new { StudentId = 3, CourseId = 4 },
                new { StudentId = 3, CourseId = 5 },
                new { StudentId = 4, CourseId = 1 },
                new { StudentId = 4, CourseId = 2 },
                new { StudentId = 5, CourseId = 1 },
                new { StudentId = 5, CourseId = 2 },
                new { StudentId = 5, CourseId = 3 },
                new { StudentId = 5, CourseId = 5 }
            );

            modelBuilder.Entity<Homework>().HasData(
                new
                {
                    HomeworkId = 1,
                    Content = "https://example.com/ahmed-csharp-assignment1.zip",
                    ContentType = ContentType.Zip,
                    SubmissionTime = new DateTime(2024, 3, 15, 23, 45, 0),
                    StudentId = 1,
                    CourseId = 1
                },
                new
                {
                    HomeworkId = 2,
                    Content = "https://example.com/fatma-csharp-assignment1.pdf",
                    ContentType = ContentType.Pdf,
                    SubmissionTime = new DateTime(2024, 3, 14, 20, 30, 0),
                    StudentId = 4,
                    CourseId = 1
                },
                new
                {
                    HomeworkId = 3,
                    Content = "https://example.com/sara-aspnet-project.zip",
                    ContentType = ContentType.Zip,
                    SubmissionTime = new DateTime(2024, 4, 20, 18, 15, 0),
                    StudentId = 2,
                    CourseId = 2
                },
                new
                {
                    HomeworkId = 4,
                    Content = "https://example.com/youssef-web-app.zip",
                    ContentType = ContentType.Zip,
                    SubmissionTime = new DateTime(2024, 4, 22, 22, 00, 0),
                    StudentId = 5,
                    CourseId = 2
                },
                new
                {
                    HomeworkId = 5,
                    Content = "https://example.com/ahmed-sql-queries.pdf",
                    ContentType = ContentType.Pdf,
                    SubmissionTime = new DateTime(2024, 4, 1, 16, 30, 0),
                    StudentId = 1,
                    CourseId = 3
                },
                new
                {
                    HomeworkId = 6,
                    Content = "https://example.com/sara-database-design.zip",
                    ContentType = ContentType.Zip,
                    SubmissionTime = new DateTime(2024, 4, 5, 14, 45, 0),
                    StudentId = 2,
                    CourseId = 3
                },
                new
                {
                    HomeworkId = 7,
                    Content = "https://example.com/omar-js-calculator",
                    ContentType = ContentType.Application,
                    SubmissionTime = new DateTime(2024, 5, 15, 19, 20, 0),
                    StudentId = 3,
                    CourseId = 4
                },
                new
                {
                    HomeworkId = 8,
                    Content = "https://example.com/omar-react-todo-app.zip",
                    ContentType = ContentType.Zip,
                    SubmissionTime = new DateTime(2024, 6, 20, 21, 10, 0),
                    StudentId = 3,
                    CourseId = 5
                },
                new
                {
                    HomeworkId = 9,
                    Content = "https://example.com/youssef-react-portfolio.zip",
                    ContentType = ContentType.Zip,
                    SubmissionTime = new DateTime(2024, 6, 25, 23, 30, 0),
                    StudentId = 5,
                    CourseId = 5
                }
            );
        }
    }
}