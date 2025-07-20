using P01_StudentSystem.Types;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P01_StudentSystem.Models
{
    public class Resource
    {
        public int ResourceId { get; set; }

        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        public string Url { get; set; } = string.Empty;

        public ResourceType ResourceType { get; set; }

        [ForeignKey(nameof(Course))]
        public int CourseId { get; set; }
        public Course Course { get; set; } = new Course();
    }
}
