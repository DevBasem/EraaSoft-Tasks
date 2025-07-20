using System.ComponentModel.DataAnnotations;

namespace P02_SalesDatabase.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(80)]
        public string Email { get; set; } = null!;

        [Required]
        public string CreditCardNumber { get; set; } = null!;
        public virtual ICollection<Sale> Sales { get; set; } = new HashSet<Sale>();
    }
}
