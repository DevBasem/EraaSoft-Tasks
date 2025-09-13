using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using MovieApp.DataAccess;
using MovieApp.Models;
using MovieApp.Services.Email;
using MovieApp.Utilities;
using MovieApp.ViewModels.Identity;
using System.Text;
using System.Text.Encodings.Web;

namespace MovieApp.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IExtendedEmailSender _emailSender;
        private readonly ILogger<AccountController> _logger;
        private readonly MoviesDbContext _dbContext;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IExtendedEmailSender emailSender,
            ILogger<AccountController> logger,
            MoviesDbContext dbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            // If user is already authenticated, redirect to home page
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home", new { area = "Public" });
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Action(
                        "ConfirmEmail",
                        "Account",
                        values: new { area = "Identity", userId = user.Id, code = code },
                        protocol: Request.Scheme);

                    // Send email using our new template
                    await _emailSender.SendEmailConfirmationAsync(
                        model.Email,
                        $"{model.FirstName} {model.LastName}",
                        callbackUrl);

                    ToastNotification.Success(TempData, "Your account was created successfully. Please check your email for confirmation instructions.");
                    
                    // Fix: Explicitly specify the area to be "Identity" in the redirect
                    return RedirectToAction("RegisterConfirmation", "Account", new { area = "Identity", email = model.Email });
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                
                ToastNotification.Error(TempData, "There was an error creating your account. Please try again.");
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult RegisterConfirmation(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Index", "Home", new { area = "Public" });
            }

            ViewBag.Email = email;
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction("Index", "Home", new { area = "Public" });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            
            if (result.Succeeded)
            {
                ToastNotification.Success(TempData, "Thank you for confirming your email. You can now log in to your account.");
            }
            else
            {
                ToastNotification.Error(TempData, "Error confirming your email. The link may have expired or is invalid.");
            }
            
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            // If user is already authenticated, redirect to home page
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home", new { area = "Public" });
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie
            await _signInManager.SignOutAsync();
            
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null, bool resendConfirmation = false, string email = null)
        {
            returnUrl ??= Url.Content("~/");
            ViewData["ReturnUrl"] = returnUrl;
            
            // Handle resend confirmation request
            if (resendConfirmation)
            {
                // If email is provided directly in the separate form, use it
                if (!string.IsNullOrEmpty(email))
                {
                    return await ResendConfirmationEmail(email);
                }
                // Otherwise try to use the email from the model
                else if (!string.IsNullOrEmpty(model.Email))
                {
                    return await ResendConfirmationEmail(model.Email);
                }
                else
                {
                    ToastNotification.Error(TempData, "Email address is required to resend confirmation.");
                    return RedirectToAction("Login", "Account", new { area = "Identity" });
                }
            }
            
            if (ModelState.IsValid)
            {
                // Check if email exists and isn't confirmed
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null && !await _userManager.IsEmailConfirmedAsync(user))
                {
                    // Check password first to make sure it's the correct user
                    var isPasswordValid = await _userManager.CheckPasswordAsync(user, model.Password);
                    if (isPasswordValid)
                    {
                        // Set TempData to indicate the user needs to confirm email
                        TempData["RequiresEmailConfirmation"] = true;
                        TempData["UserEmail"] = model.Email;
                        
                        ToastNotification.Warning(TempData, "Your email hasn't been confirmed yet. Please check your email or request a new confirmation link.");
                        return View(model);
                    }
                }

                // Try to sign in the user
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    ToastNotification.Success(TempData, "You have successfully logged in.");
                    return LocalRedirect(returnUrl);
                }
                
                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction("LoginWith2fa", "Account", new { area = "Identity", ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                }
                
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    ToastNotification.Error(TempData, "Your account has been locked out. Please try again later.");
                    return RedirectToAction("Lockout", "Account", new { area = "Identity" });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            return View(model);
        }
        
        private async Task<IActionResult> ResendConfirmationEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ToastNotification.Error(TempData, "Email address is required.");
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }
            
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // Don't reveal that the user doesn't exist
                ToastNotification.Success(TempData, "If your email is registered, a confirmation link has been sent. Please check your inbox.");
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }
            
            if (await _userManager.IsEmailConfirmedAsync(user))
            {
                ToastNotification.Info(TempData, "Your email is already confirmed. You can log in to your account.");
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }
            
            // Generate the confirmation code and send email
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Action(
                "ConfirmEmail",
                "Account",
                values: new { area = "Identity", userId = user.Id, code = code },
                protocol: Request.Scheme);

            // Send the email with our new template
            await _emailSender.SendEmailConfirmationAsync(
                email, 
                $"{user.FirstName} {user.LastName}",
                callbackUrl);

            ToastNotification.Success(TempData, "Verification email sent. Please check your inbox.");
            
            // Keep the email confirmation status for the view
            TempData["RequiresEmailConfirmation"] = true;
            TempData["UserEmail"] = email;
            
            return RedirectToAction("Login", "Account", new { area = "Identity" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            ToastNotification.Info(TempData, "You have been logged out successfully.");
            return RedirectToAction("Index", "Home", new { area = "Public" });
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            // If user is already authenticated, redirect to home page
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home", new { area = "Public" });
            }

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                
                // Check if user exists
                if (user == null)
                {
                    // Don't reveal that the user does not exist
                    ToastNotification.Info(TempData, "If your email is registered and confirmed, you will receive a verification code.");
                    return RedirectToAction("ForgotPasswordConfirmation", "Account", new { area = "Identity" });
                }

                // Check if email is confirmed
                if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    // If email is not confirmed, redirect to a special page where they can verify their email first
                    TempData["UnverifiedEmail"] = model.Email;
                    ToastNotification.Warning(TempData, "You need to verify your email address before you can reset your password.");
                    
                    // Send a new confirmation email
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Action(
                        "ConfirmEmail",
                        "Account",
                        values: new { area = "Identity", userId = user.Id, code = code },
                        protocol: Request.Scheme);

                    // Send the email with our template
                    await _emailSender.SendEmailConfirmationAsync(
                        user.Email,
                        $"{user.FirstName} {user.LastName}",
                        callbackUrl);
                        
                    // Redirect to a view that tells the user to verify their email first
                    return View("VerifyEmailFirst", model.Email);
                }

                // Generate 4-digit OTP
                var otpCode = GenerateOtpCode(4);
                
                // Save OTP in the database
                var passwordResetOtp = new PasswordResetOtp
                {
                    UserId = user.Id,
                    Email = user.Email,
                    OtpCode = otpCode,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                    IsUsed = false
                };
                
                // Remove any existing OTPs for this user
                var existingOtps = _dbContext.PasswordResetOtps.Where(o => o.UserId == user.Id);
                _dbContext.PasswordResetOtps.RemoveRange(existingOtps);
                
                // Add the new OTP
                await _dbContext.PasswordResetOtps.AddAsync(passwordResetOtp);
                await _dbContext.SaveChangesAsync();

                // Send OTP to user's email
                await _emailSender.SendPasswordResetOtpAsync(
                    user.Email,
                    $"{user.FirstName} {user.LastName}",
                    otpCode);

                return RedirectToAction("VerifyOtp", "Account", new { area = "Identity", email = model.Email });
            }

            return View(model);
        }
        
        private string GenerateOtpCode(int length)
        {
            var random = new Random();
            var otp = random.Next((int)Math.Pow(10, length - 1), (int)Math.Pow(10, length) - 1).ToString();
            return otp;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult VerifyOtp(string email)
        {
            // If user is already authenticated, redirect to home page
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home", new { area = "Public" });
            }

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("ForgotPassword", "Account", new { area = "Identity" });
            }
            
            var model = new VerifyOtpViewModel
            {
                Email = email
            };
            
            return View(model);
        }
        
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyOtp(VerifyOtpViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            // Find the OTP in the database
            var otp = _dbContext.PasswordResetOtps
                .FirstOrDefault(o => o.Email == model.Email && 
                                    o.OtpCode == model.OtpCode && 
                                    o.ExpiresAt > DateTime.UtcNow &&
                                    !o.IsUsed);
            
            if (otp == null)
            {
                ModelState.AddModelError("OtpCode", "Invalid or expired verification code.");
                return View(model);
            }
            
            // Get user
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User not found.");
                return View(model);
            }
            
            // Reset password
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
            
            // Mark OTP as used
            otp.IsUsed = true;
            await _dbContext.SaveChangesAsync();
            
            ToastNotification.Success(TempData, "Your password has been reset successfully.");
            return RedirectToAction("ResetPasswordConfirmation", "Account", new { area = "Identity" });
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}