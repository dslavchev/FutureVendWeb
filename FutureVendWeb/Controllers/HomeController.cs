using System.Diagnostics;
using FutureVendWeb.Data.Models;
using FutureVendWeb.Data.Models.User;
using FutureVendWeb.Services.User;
using Microsoft.AspNetCore.Mvc;

namespace FutureVendWeb.Controllers
{
    /// <summary>
    /// The HomeController class is responsible for handling requests to the home page and privacy page of the application.
    /// It also manages error handling and displays the corresponding views.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly IUserService _userService;
        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        
        public HomeController(IUserService userService)
        {
            _userService = userService;
            // Constructor logic can be added here if needed.
        }

        /// <summary>
        /// Displays the main index page of the application.
        /// This action is protected and requires the user to be authenticated.
        /// </summary>
        /// <returns>The view for the Index page.</returns>
        public IActionResult Index()
        {
            UserData? user = _userService.GetUser();
            if (user == null) return RedirectToAction("Login", "User");

            return View();
        }

       

        /// <summary>
        /// Displays the privacy page of the application.
        /// </summary>
        /// <returns>The view for the Privacy page.</returns>
        public IActionResult Privacy()
        {
            UserData? user = _userService.GetUser();
            if (user == null) return RedirectToAction("Login", "User");

            return View();
        }

        /// <summary>
        /// Handles errors in the application by displaying the error view.
        /// The error view includes the request ID for debugging purposes.
        /// This action is configured to not cache the response.
        /// </summary>
        /// <returns>The view for the Error page with an ErrorViewModel that includes the request ID.</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
