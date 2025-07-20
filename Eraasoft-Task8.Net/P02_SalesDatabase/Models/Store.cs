using System.ComponentModel.DataAnnotations;

namespace P02_SalesDatabase.Models
{
    public class Store
    {
        [Key]
        public int StoreId { get; set; }

        [Required]
        [MaxLength(80)]
        public string Name { get; set; } = null!;
        public virtual ICollection<Sale> Sales { get; set; } = new HashSet<Sale>();
    }
}
