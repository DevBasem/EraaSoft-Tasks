using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using MovieApp.Models;
using Microsoft.Extensions.Logging;

namespace MovieApp.Middleware
{
    public class AdminAreaProtectionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AdminAreaProtectionMiddleware> _logger;

        public AdminAreaProtectionMiddleware(RequestDelegate next, ILogger<AdminAreaProtectionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, UserManager<ApplicationUser> userManager)
        {
            var path = context.Request.Path.Value?.ToLowerInvariant();
            
            // Skip middleware processing for static files and favicon
            if (path == null || 
                path.StartsWith("/lib/") ||
                path.StartsWith("/css/") ||
                path.StartsWith("/js/") ||
                path.StartsWith("/images/") ||
                path == "/favicon.ico")
            {
                await _next(context);
                return;
            }
            
            // Only protect Admin area paths
            if (path.StartsWith("/admin"))
            {
                // Check if the user is authenticated
                if (!context.User.Identity.IsAuthenticated)
                {
                    var returnUrl = context.Request.Path + context.Request.QueryString;
                    context.Response.Redirect($"/Identity/Account/Login?ReturnUrl={Uri.EscapeDataString(returnUrl)}");
                    return;
                }
                
                // Check if user is in Admin role
                if (!context.User.IsInRole("Admin"))
                {
                    context.Response.Redirect("/Identity/Account/AccessDenied");
                    return;
                }
            }
            
            // Continue with the request pipeline
            await _next(context);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline
    public static class AdminAreaProtectionMiddlewareExtensions
    {
        public static IApplicationBuilder UseAdminAreaProtection(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AdminAreaProtectionMiddleware>();
        }
    }
}