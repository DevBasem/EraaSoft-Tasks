using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MovieApp.Models;

namespace MovieApp.DataAccess
{
    public class IdentityDataSeeder
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<IdentityDataSeeder> _logger;

        public IdentityDataSeeder(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<IdentityDataSeeder> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task SeedData()
        {
            try
            {
                // Create roles if they don't exist
                if (!await _roleManager.RoleExistsAsync("Admin"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("Admin"));
                    _logger.LogInformation("Admin role created");
                }

                if (!await _roleManager.RoleExistsAsync("User"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("User"));
                    _logger.LogInformation("User role created");
                }

                // Create admin user if it doesn't exist
                var adminEmail = "admin@movieapp.com";
                var adminUser = await _userManager.FindByEmailAsync(adminEmail);

                if (adminUser == null)
                {
                    adminUser = new ApplicationUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true,
                        FirstName = "Admin",
                        LastName = "User",
                        CreatedOn = DateTime.Now,
                        IsActive = true
                    };

                    var result = await _userManager.CreateAsync(adminUser, "Admin123!");

                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(adminUser, "Admin");
                        _logger.LogInformation("Admin user created");
                    }
                    else
                    {
                        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                        _logger.LogError("Error creating admin user: {Errors}", errors);
                    }
                }
                else if (!await _userManager.IsInRoleAsync(adminUser, "Admin"))
                {
                    await _userManager.AddToRoleAsync(adminUser, "Admin");
                    _logger.LogInformation("Added Admin role to existing admin user");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding identity data");
            }
        }
    }
}