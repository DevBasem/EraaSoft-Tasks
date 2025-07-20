using System.ComponentModel.DataAnnotations;

namespace P01_StudentSystem.Models
{
    public class Student
    {
        public int StudentId { get; set; }

        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(10)]
        public string? PhoneNumber { get; set; }

        public DateTime RegisteredOn { get; set; }

        public DateOnly? Birthday { get; set; }

        public List<Course> Courses { get; set; } = new List<Course>();
        public List<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
        public List<Homework> HomeworkSubmissions { get; set; } = new List<Homework>();
    }
}
