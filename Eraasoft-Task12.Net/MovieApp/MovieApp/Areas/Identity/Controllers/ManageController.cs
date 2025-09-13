using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MovieApp.Models;
using MovieApp.Utilities;
using MovieApp.ViewModels.Identity;
using System.Threading.Tasks;

namespace MovieApp.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Authorize]
    public class ManageController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<ManageController> _logger;
        private readonly string _superAdminEmail = "admin@movieapp.com";

        public ManageController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<ManageController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var model = new ProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                IsEmailConfirmed = user.EmailConfirmed,
                PhoneNumber = user.PhoneNumber,
                IsSuperAdmin = user.Email.Equals(_superAdminEmail, StringComparison.OrdinalIgnoreCase)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check if this is the super admin account
            bool isSuperAdmin = user.Email.Equals(_superAdminEmail, StringComparison.OrdinalIgnoreCase);
            model.IsSuperAdmin = isSuperAdmin;

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            await _signInManager.RefreshSignInAsync(user);
            ToastNotification.Success(TempData, "Your profile has been updated");
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            // Prevent super admin from changing password
            if (user.Email.Equals(_superAdminEmail, StringComparison.OrdinalIgnoreCase))
            {
                ToastNotification.Error(TempData, "Super admin password cannot be changed for security reasons.");
                return RedirectToAction(nameof(Index));
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (!hasPassword)
            {
                return RedirectToAction(nameof(SetPassword));
            }

            return View(new ChangePasswordViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            // Prevent super admin from changing password
            if (user.Email.Equals(_superAdminEmail, StringComparison.OrdinalIgnoreCase))
            {
                ToastNotification.Error(TempData, "Super admin password cannot be changed for security reasons.");
                return RedirectToAction(nameof(Index));
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            await _signInManager.RefreshSignInAsync(user);
            _logger.LogInformation("User changed their password successfully.");
            ToastNotification.Success(TempData, "Your password has been changed.");

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> SetPassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            // Prevent super admin from changing password
            if (user.Email.Equals(_superAdminEmail, StringComparison.OrdinalIgnoreCase))
            {
                ToastNotification.Error(TempData, "Super admin password cannot be changed for security reasons.");
                return RedirectToAction(nameof(Index));
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (hasPassword)
            {
                return RedirectToAction(nameof(ChangePassword));
            }

            return View(new SetPasswordViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            // Prevent super admin from changing password
            if (user.Email.Equals(_superAdminEmail, StringComparison.OrdinalIgnoreCase))
            {
                ToastNotification.Error(TempData, "Super admin password cannot be changed for security reasons.");
                return RedirectToAction(nameof(Index));
            }

            var addPasswordResult = await _userManager.AddPasswordAsync(user, model.NewPassword);
            if (!addPasswordResult.Succeeded)
            {
                foreach (var error in addPasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            await _signInManager.RefreshSignInAsync(user);
            ToastNotification.Success(TempData, "Your password has been set.");

            return RedirectToAction(nameof(Index));
        }
    }
}