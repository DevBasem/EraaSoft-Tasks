using System.ComponentModel.DataAnnotations;

namespace P01_StudentSystem.Models
{
    public class Course
    {
        public int CourseId { get; set; }

        [MaxLength(80)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public decimal Price { get; set; }

        public List<Student> Students { get; set; } = new List<Student>();
        public List<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
        public List<Resource> Resources { get; set; } = new List<Resource>();
        public List<Homework> HomeworkSubmissions { get; set; } = new List<Homework>();
    }
}
