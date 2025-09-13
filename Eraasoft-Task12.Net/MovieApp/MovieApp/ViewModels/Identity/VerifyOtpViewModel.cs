using System.ComponentModel.DataAnnotations;

namespace MovieApp.ViewModels.Identity
{
    public class VerifyOtpViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        [Display(Name = "Verification Code")]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "Verification code must be 4 digits.")]
        public string OtpCode { get; set; }
        
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}