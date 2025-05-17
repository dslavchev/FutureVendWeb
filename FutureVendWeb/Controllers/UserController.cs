using FutureVendWeb.Data.Models.User;
using FutureVendWeb.Services.User;
using Microsoft.AspNetCore.Mvc;

namespace FutureVendWeb.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email , string password)
        {
            UserData user;
            try
            {
                user = _userService.RegisterUser(email, password);
            }
            catch (Exception ex)
            {
                ViewData["Error"] = ex.Message;
                return View();
            }
            
            _userService.SetUser(user);

            return RedirectToAction("Index", "Home");

        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(CreateUser createUser)
        {
            _userService.CreateUser(createUser);
            _userService.SetUser(null);

            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            _userService.SetUser(null);

            return RedirectToAction("Login");
        }
    }
}
