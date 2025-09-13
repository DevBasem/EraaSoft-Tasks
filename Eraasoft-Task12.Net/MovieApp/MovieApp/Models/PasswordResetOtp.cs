using System;
using System.ComponentModel.DataAnnotations;

namespace MovieApp.Models
{
    public class PasswordResetOtp
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string UserId { get; set; }
        
        [Required]
        public string Email { get; set; }
        
        [Required]
        public string OtpCode { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime ExpiresAt { get; set; }
        
        public bool IsUsed { get; set; } = false;
    }
}