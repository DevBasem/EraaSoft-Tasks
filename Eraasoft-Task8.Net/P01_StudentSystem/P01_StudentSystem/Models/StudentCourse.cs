using System.ComponentModel.DataAnnotations.Schema;

namespace P01_StudentSystem.Models
{
    public class StudentCourse
    {
        [ForeignKey(nameof(Student))]
        public int StudentId { get; set; }
        public Student Student { get; set; } = new Student();

        [ForeignKey(nameof(Course))]
        public int CourseId { get; set; }
        public Course Course { get; set; } = new Course();
    }
}
