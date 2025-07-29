using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P01_StudentSystem.Data.Models
{
    [Table("customers")]
    public class Customer
    {
        [Key]
        [Column("customer_id")]
        public int CustomerId { get; set; }

        [Column("first_name")]
        [StringLength(255)]
        public string FirstName { get; set; } = null!;

        [Column("last_name")]
        [StringLength(255)]
        public string LastName { get; set; } = null!;

        [Column("phone")]
        [StringLength(25)]
        public string? Phone { get; set; }

        [Column("email")]
        [StringLength(255)]
        public string Email { get; set; } = null!;

        [Column("street")]
        [StringLength(255)]
        public string? Street { get; set; }

        [Column("city")]
        [StringLength(50)]
        public string? City { get; set; }

        [Column("state")]
        [StringLength(25)]
        public string? State { get; set; }

        [Column("zip_code")]
        [StringLength(5)]
        public string? ZipCode { get; set; }

        // Navigation property
        public virtual ICollection<Order> Orders { get; set; } = new HashSet<Order>();
    }
}