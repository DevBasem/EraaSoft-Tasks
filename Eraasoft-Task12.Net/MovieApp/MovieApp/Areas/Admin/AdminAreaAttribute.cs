using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MovieApp.Areas.Admin
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminAreaAttribute : AreaAttribute
    {
        public AdminAreaAttribute() : base("Admin")
        {
        }
    }
}