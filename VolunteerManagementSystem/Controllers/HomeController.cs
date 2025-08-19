using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using VolunteerManagementSystem.Models;
using VolunteerManagementSystem.Filters;
using VolunteerManagementSystem.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace VolunteerManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [AdminOnly]  // protected
        public async Task<IActionResult> Index()
        {
            ViewBag.VolunteerCount = await _context.Volunteers.CountAsync();
            ViewBag.OpportunityCount = await _context.Opportunities.CountAsync();
            return View();
        }

        [AdminOnly]  // protected
        public IActionResult Privacy() => View();

        [AllowAnonymous]  // public
        public IActionResult About()
        {
            ViewData["Title"] = "About Us";
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() =>
            View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
