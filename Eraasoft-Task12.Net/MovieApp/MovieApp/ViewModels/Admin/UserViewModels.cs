using Microsoft.AspNetCore.Mvc.Rendering;
using MovieApp.Models;
using MovieApp.Utilities;
using System.ComponentModel.DataAnnotations;

namespace MovieApp.ViewModels.Admin
{
    public class UsersViewModel
    {
        public IEnumerable<ApplicationUser> Users { get; set; }
        public MovieApp.Utilities.Pager Pager { get; set; }
    }

    public class UserDetailsViewModel
    {
        public string Id { get; set; }
        
        [Display(Name = "Email")]
        public string Email { get; set; }
        
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        
        [Display(Name = "Email Confirmed")]
        public bool EmailConfirmed { get; set; }
        
        [Display(Name = "Super Admin")]
        public bool IsSuperAdmin { get; set; }
        
        [Display(Name = "Roles")]
        public List<string> Roles { get; set; }
        
        [Display(Name = "Created On")]
        public DateTime CreatedOn { get; set; }
        
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
    }

    public class UserEditViewModel
    {
        public string Id { get; set; }
        
        [Display(Name = "Email")]
        public string Email { get; set; }
        
        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        
        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        
        [Display(Name = "Email Confirmed")]
        public bool EmailConfirmed { get; set; }
        
        [Display(Name = "Super Admin")]
        public bool IsSuperAdmin { get; set; }
        
        [Display(Name = "Admin")]
        public bool IsAdmin { get; set; }
        
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
    }

    public class UserResetPasswordViewModel
    {
        public string Id { get; set; }
        
        [Display(Name = "Email")]
        public string Email { get; set; }
        
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}