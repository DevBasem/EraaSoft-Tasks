using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieApp.Areas.Admin;
using MovieApp.Models;
using MovieApp.Utilities;
using MovieApp.ViewModels.Admin;
using System.Threading.Tasks;

namespace MovieApp.Areas.Admin.Controllers
{
    [AdminAreaAttribute]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UsersController> _logger;
        private readonly string _superAdminEmail = "admin@movieapp.com";

        public UsersController(
            UserManager<ApplicationUser> userManager,
            ILogger<UsersController> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var totalUsers = await _userManager.Users.CountAsync();
            var pager = new MovieApp.Utilities.Pager(totalUsers, page, pageSize);

            var users = await _userManager.Users
                .OrderBy(u => u.Email)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var model = new UsersViewModel
            {
                Users = users,
                Pager = pager
            };

            return View(model);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var isSuperAdmin = user.Email.Equals(_superAdminEmail, StringComparison.OrdinalIgnoreCase);
            var roles = await _userManager.GetRolesAsync(user);

            var model = new UserDetailsViewModel
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                EmailConfirmed = user.EmailConfirmed,
                IsSuperAdmin = isSuperAdmin,
                Roles = roles.ToList(),
                CreatedOn = user.CreatedOn,
                IsActive = user.IsActive
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var isSuperAdmin = user.Email.Equals(_superAdminEmail, StringComparison.OrdinalIgnoreCase);
            var roles = await _userManager.GetRolesAsync(user);

            var model = new UserEditViewModel
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                EmailConfirmed = user.EmailConfirmed,
                IsSuperAdmin = isSuperAdmin,
                IsAdmin = roles.Contains("Admin"),
                IsActive = user.IsActive
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return NotFound();
            }

            // Check if this is the super admin
            var isSuperAdmin = user.Email.Equals(_superAdminEmail, StringComparison.OrdinalIgnoreCase);

            // Don't allow changing the super admin's active status
            if (isSuperAdmin && !model.IsActive)
            {
                ToastNotification.Error(TempData, "Cannot deactivate super admin account.");
                model.IsActive = true;
                return View(model);
            }

            // Update user fields
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;
            user.EmailConfirmed = model.EmailConfirmed;
            user.IsActive = model.IsActive;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            // Handle Admin role assignment except for super admin
            if (!isSuperAdmin)
            {
                var isInAdminRole = await _userManager.IsInRoleAsync(user, "Admin");

                if (model.IsAdmin && !isInAdminRole)
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                }
                else if (!model.IsAdmin && isInAdminRole)
                {
                    await _userManager.RemoveFromRoleAsync(user, "Admin");
                }
            }

            ToastNotification.Success(TempData, "User updated successfully.");
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Don't allow resetting super admin password
            if (user.Email.Equals(_superAdminEmail, StringComparison.OrdinalIgnoreCase))
            {
                ToastNotification.Error(TempData, "Super admin password cannot be reset for security reasons.");
                return RedirectToAction(nameof(Details), new { id });
            }

            var model = new UserResetPasswordViewModel
            {
                Id = user.Id,
                Email = user.Email
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(UserResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return NotFound();
            }

            // Don't allow resetting super admin password
            if (user.Email.Equals(_superAdminEmail, StringComparison.OrdinalIgnoreCase))
            {
                ToastNotification.Error(TempData, "Super admin password cannot be reset for security reasons.");
                return RedirectToAction(nameof(Details), new { id = model.Id });
            }

            // Generate reset token and reset the password
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            ToastNotification.Success(TempData, "Password reset successfully.");
            return RedirectToAction(nameof(Details), new { id = model.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Don't allow deactivating super admin
            if (user.Email.Equals(_superAdminEmail, StringComparison.OrdinalIgnoreCase))
            {
                ToastNotification.Error(TempData, "Cannot change status of super admin account.");
                return RedirectToAction(nameof(Index));
            }

            user.IsActive = !user.IsActive;
            await _userManager.UpdateAsync(user);

            string statusMessage = user.IsActive ? "activated" : "deactivated";
            ToastNotification.Success(TempData, $"User {statusMessage} successfully.");

            return RedirectToAction(nameof(Index));
        }
    }
}