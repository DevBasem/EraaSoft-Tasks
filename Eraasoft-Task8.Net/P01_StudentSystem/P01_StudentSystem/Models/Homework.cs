using P01_StudentSystem.Types;
using System.ComponentModel.DataAnnotations.Schema;

namespace P01_StudentSystem.Models
{
    public class Homework
    {
        public int HomeworkId { get; set; }

        public string Content { get; set; } = string.Empty;

        public ContentType ContentType { get; set; }

        public DateTime SubmissionTime { get; set; }

        [ForeignKey(nameof(Student))]
        public int StudentId { get; set; }
        public Student Student { get; set; } = new Student();

        [ForeignKey(nameof(Course))]
        public int CourseId { get; set; }
        public Course Course { get; set; } = new Course();
    }
}
