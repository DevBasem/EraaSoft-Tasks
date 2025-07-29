using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P01_StudentSystem.Data.Models
{
    [Table("brands")]
    public class Brand
    {
        [Key]
        [Column("brand_id")]
        public int BrandId { get; set; }

        [Column("brand_name")]
        [StringLength(255)]
        public string BrandName { get; set; } = null!;

        // Navigation property
        public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();
    }
}