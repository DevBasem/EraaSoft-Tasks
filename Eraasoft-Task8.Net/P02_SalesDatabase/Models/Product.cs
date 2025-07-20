using System.ComponentModel.DataAnnotations;

namespace P02_SalesDatabase.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = null!;

        public decimal Quantity { get; set; }

        public decimal Price { get; set; }

        [MaxLength(250)]
        public string Description { get; set; } = "No description";
        public virtual ICollection<Sale> Sales { get; set; } = new HashSet<Sale>();
    }
}
