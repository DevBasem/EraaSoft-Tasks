using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using MovieApp.DataAccess;
using MovieApp.Middleware;
using MovieApp.Models;
using MovieApp.Repositories;
using MovieApp.Repositories.IRepositories;
using MovieApp.Services;
using MovieApp.Services.Email;
using MovieApp.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Configure database context
builder.Services.AddDbContext<MoviesDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => {
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = true;
})
.AddEntityFrameworkStores<MoviesDbContext>()
.AddDefaultTokenProviders();

// Configure Email services
var emailConfig = builder.Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig);
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IEmailTemplateRenderer, EmailTemplateRenderer>();
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddTransient<IExtendedEmailSender, EmailSender>();

// Add the Identity data seeder
builder.Services.AddScoped<IdentityDataSeeder>();

// Configure application cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(1);
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});

// Register repositories
builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICinemaRepository, CinemaRepository>();
builder.Services.AddScoped<IActorRepository, ActorRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register services
builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IActorService, ActorService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICinemaService, CinemaService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IPublicMovieService, PublicMovieService>();
builder.Services.AddScoped<IHomeService, HomeService>();

var app = builder.Build();

// Apply migrations and seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<MoviesDbContext>();
        context.Database.Migrate();
        
        // Seed identity data
        var seeder = services.GetRequiredService<IdentityDataSeeder>();
        await seeder.SeedData();
        
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Database migration and seeding completed successfully");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while applying migrations or seeding data");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
else
{
    // In development, still use custom error pages but with more details
    app.UseExceptionHandler("/Error");
}

// Add status code pages middleware for handling 404, 500, etc.
app.UseStatusCodePagesWithReExecute("/Error/{0}");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Add authentication middleware
app.UseAuthentication();
app.UseAuthorization();

// Redirect authenticated users away from authentication pages
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value?.ToLowerInvariant();
    
    // Check if user is authenticated and trying to access auth pages
    if (context.User.Identity.IsAuthenticated && path != null)
    {
        // List of authentication paths that authenticated users shouldn't access
        var authPaths = new[]
        {
            "/identity/account/login",
            "/identity/account/register",
            "/identity/account/forgotpassword",
            "/identity/account/lockout"
        };
        
        if (authPaths.Any(authPath => path.StartsWith(authPath)))
        {
            context.Response.Redirect("/Public/Home/Index");
            return;
        }
    }
    
    await next();
});

// Add custom middleware to protect Admin area
app.UseAdminAreaProtection();

// Special case for the root /Admin path
app.MapGet("/Admin", context => {
    if (!context.User.Identity.IsAuthenticated)
    {
        context.Response.Redirect("/Identity/Account/Login?ReturnUrl=%2FAdmin");
        return Task.CompletedTask;
    }
    else if (!context.User.IsInRole("Admin"))
    {
        context.Response.Redirect("/Identity/Account/AccessDenied");
        return Task.CompletedTask;
    }
    
    // If they are authenticated and an admin, redirect to the dashboard
    context.Response.Redirect("/Admin/Dashboard/Index");
    return Task.CompletedTask;
});

// Route configuration
// First specific admin route with Dashboard as default controller
app.MapControllerRoute(
    name: "admin",
    pattern: "Admin/{controller=Dashboard}/{action=Index}/{id?}",
    defaults: new { area = "Admin" });


// General area route - includes all areas
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// Default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// Add a redirect from root to Public/Home/Index
app.MapGet("/", context => {
    context.Response.Redirect("/Public/Home/Index");
    return Task.CompletedTask;
});

app.Run();
