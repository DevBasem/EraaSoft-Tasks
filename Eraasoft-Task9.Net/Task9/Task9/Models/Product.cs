using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P01_StudentSystem.Data.Models
{
    [Table("products")]
    public class Product
    {
        [Key]
        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("product_name")]
        [StringLength(255)]
        public string ProductName { get; set; } = null!;

        [Column("brand_id")]
        public int BrandId { get; set; }

        [Column("category_id")]
        public int CategoryId { get; set; }

        [Column("model_year")]
        public short ModelYear { get; set; }

        [Column("list_price", TypeName = "decimal(10,2)")]
        public decimal ListPrice { get; set; }

        // Navigation properties
        [ForeignKey("BrandId")]
        public virtual Brand Brand { get; set; } = null!;

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; } = null!;

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new HashSet<OrderItem>();
        public virtual ICollection<Stock> Stocks { get; set; } = new HashSet<Stock>();
    }
}