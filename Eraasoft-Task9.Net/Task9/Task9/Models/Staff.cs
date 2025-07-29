using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P01_StudentSystem.Data.Models
{
    [Table("staffs")]
    public class Staff
    {
        [Key]
        [Column("staff_id")]
        public int StaffId { get; set; }

        [Column("first_name")]
        [StringLength(50)]
        public string FirstName { get; set; } = null!;

        [Column("last_name")]
        [StringLength(50)]
        public string LastName { get; set; } = null!;

        [Column("email")]
        [StringLength(255)]
        public string Email { get; set; } = null!;

        [Column("phone")]
        [StringLength(25)]
        public string? Phone { get; set; }

        [Column("active")]
        public byte Active { get; set; }

        [Column("store_id")]
        public int StoreId { get; set; }

        [Column("manager_id")]
        public int? ManagerId { get; set; }

        // Navigation properties
        [ForeignKey("StoreId")]
        public virtual Store Store { get; set; } = null!;

        [ForeignKey("ManagerId")]
        public virtual Staff? Manager { get; set; }

        public virtual ICollection<Staff> ManagedStaffs { get; set; } = new HashSet<Staff>();
        public virtual ICollection<Order> Orders { get; set; } = new HashSet<Order>();
    }
}