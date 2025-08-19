using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VolunteerManagementSystem.Data;
using VolunteerManagementSystem.Filters;
using VolunteerManagementSystem.Models;

namespace VolunteerManagementSystem.Controllers
{
    [AdminOnly]
    public class VolunteersController : Controller
    {
        private readonly AppDbContext _context;

        public VolunteersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Volunteers
        public async Task<IActionResult> Index(string? status, string? q)
        {
            var query = _context.Volunteers.AsQueryable();

            // Filter by ApprovalStatus
            if (!string.IsNullOrWhiteSpace(status) &&
                Enum.TryParse<ApprovalStatus>(status, out var s))
            {
                query = query.Where(v => v.ApprovalStatus == s);
            }

            // Search by name or email
            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(v =>
                    v.FirstName.Contains(q) ||
                    v.LastName.Contains(q) ||
                    v.Email.Contains(q));
            }

            ViewBag.Status = status;
            ViewBag.Query = q;

            var list = await query
                .AsNoTracking()
                .OrderBy(v => v.LastName)
                .ThenBy(v => v.FirstName)
                .ToListAsync();

            return View(list);
        }

        // GET: Volunteers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Volunteer not found.";
                return RedirectToAction(nameof(Index));
            }

            var volunteer = await _context.Volunteers
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (volunteer == null)
            {
                TempData["Error"] = "Volunteer not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(volunteer);
        }

        // GET: Volunteers/Create
        public IActionResult Create() => View();

        // POST: Volunteers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Email,ApprovalStatus")] Volunteer volunteer)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please fix the errors and try again.";
                return View(volunteer);
            }

            _context.Add(volunteer);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Volunteer created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Volunteers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Volunteer not found.";
                return RedirectToAction(nameof(Index));
            }

            var volunteer = await _context.Volunteers.FindAsync(id);
            if (volunteer == null)
            {
                TempData["Error"] = "Volunteer not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(volunteer);
        }

        // POST: Volunteers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Email,ApprovalStatus")] Volunteer volunteer)
        {
            if (id != volunteer.Id)
            {
                TempData["Error"] = "Volunteer mismatch.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please fix the errors and try again.";
                return View(volunteer);
            }

            try
            {
                _context.Update(volunteer);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Volunteer updated.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Volunteers.Any(e => e.Id == volunteer.Id))
                {
                    TempData["Error"] = "Volunteer no longer exists.";
                    return RedirectToAction(nameof(Index));
                }
                throw; // let dev page show details
            }
        }

        // GET: Volunteers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Volunteer not found.";
                return RedirectToAction(nameof(Index));
            }

            var volunteer = await _context.Volunteers
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (volunteer == null)
            {
                TempData["Error"] = "Volunteer not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(volunteer);
        }

        // POST: Volunteers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var volunteer = await _context.Volunteers.FindAsync(id);
            if (volunteer == null)
            {
                TempData["Error"] = "Volunteer not found.";
                return RedirectToAction(nameof(Index));
            }

            _context.Volunteers.Remove(volunteer);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Volunteer deleted.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Volunteers/Matches/5  (stub: show 10 most recent opportunities)
        public async Task<IActionResult> Matches(int id)
        {
            var volunteer = await _context.Volunteers.FindAsync(id);
            if (volunteer == null)
            {
                TempData["Error"] = "Volunteer not found.";
                return RedirectToAction(nameof(Index));
            }

            var opportunities = await _context.Opportunities
                .AsNoTracking()
                .OrderByDescending(o => o.CreatedDate)
                .Take(10)
                .ToListAsync();

            ViewBag.Volunteer = volunteer;
            return View(opportunities);
        }
    }
}
