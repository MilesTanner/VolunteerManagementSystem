using Microsoft.AspNetCore.Mvc;

namespace VolunteerManagementSystem.Controllers
{
    public class AuthController : Controller
    {
        private readonly IConfiguration _cfg;
        public AuthController(IConfiguration cfg) => _cfg = cfg;

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password, string? returnUrl = null)
        {
            var u = _cfg["AdminLogin:Username"];
            var p = _cfg["AdminLogin:Password"];

            if (username == u && password == p)
            {
                HttpContext.Session.SetString("Admin", "true");
                if (!string.IsNullOrWhiteSpace(returnUrl)) return Redirect(returnUrl);
                return RedirectToAction("Index", "Home"); // change to Volunteers later if you want
            }

            ViewBag.Error = "Invalid credentials";
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
