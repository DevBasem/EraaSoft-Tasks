using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApp.Areas.Admin;

namespace MovieApp.Areas.Admin.Controllers
{
    [AdminArea]
    public class ErrorController : Controller
    {
        [Route("Admin/Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMessage = "The admin page you are looking for could not be found";
                    ViewBag.StatusCode = statusCode;
                    break;
                case 403:
                    ViewBag.ErrorMessage = "You don't have permission to access this admin resource";
                    ViewBag.StatusCode = statusCode;
                    break;
                default:
                    ViewBag.ErrorMessage = "An error occurred in the admin area";
                    ViewBag.StatusCode = statusCode;
                    break;
            }

            return View("NotFound");
        }
    }
}