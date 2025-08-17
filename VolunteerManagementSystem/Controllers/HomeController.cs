using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using VolunteerManagementSystem.Models;
using VolunteerManagementSystem.Filters;
using VolunteerManagementSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace VolunteerManagementSystem.Controllers
{
    [AdminOnly] // ensures Home is locked behind login
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.VolunteerCount = await _context.Volunteers.CountAsync();
            ViewBag.OpportunityCount = await _context.Opportunities.CountAsync();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
