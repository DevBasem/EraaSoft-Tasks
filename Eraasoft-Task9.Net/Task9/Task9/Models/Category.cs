using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P01_StudentSystem.Data.Models
{
    [Table("categories")]
    public class Category
    {
        [Key]
        [Column("category_id")]
        public int CategoryId { get; set; }

        [Column("category_name")]
        [StringLength(255)]
        public string CategoryName { get; set; } = null!;

        // Navigation property
        public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();
    }
}