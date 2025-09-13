using Microsoft.AspNetCore.Mvc;

namespace MovieApp.Areas.Public.Controllers
{
    [Area("Public")]
    public class ErrorController : Controller
    {
        [Route("Public/Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMessage = "The page you are looking for could not be found";
                    ViewBag.StatusCode = statusCode;
                    break;
                default:
                    ViewBag.ErrorMessage = "An error occurred while processing your request";
                    ViewBag.StatusCode = statusCode;
                    break;
            }

            return View("NotFound");
        }
    }
}