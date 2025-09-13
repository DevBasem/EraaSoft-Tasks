using Microsoft.AspNetCore.Identity;
using System;

namespace MovieApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
    }
}