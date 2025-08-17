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
                query = query.Where(v =>
                    v.FirstName.Contains(q) ||
                    v.LastName.Contains(q) ||
                    v.Email.Contains(q));
            }

            ViewBag.Status = status;
            ViewBag.Query = q;

            var list = await query
                .OrderBy(v => v.LastName)
                .ThenBy(v => v.FirstName)
                .ToListAsync();

            return View(list);
        }

        // GET: Volunteers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var volunteer = await _context.Volunteers
                .FirstOrDefaultAsync(m => m.Id == id);

            if (volunteer == null) return NotFound();

            return View(volunteer);
        }

        // GET: Volunteers/Create
        public IActionResult Create() => View();

        // POST: Volunteers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Email,ApprovalStatus")] Volunteer volunteer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(volunteer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(volunteer);
        }

        // GET: Volunteers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var volunteer = await _context.Volunteers.FindAsync(id);
            if (volunteer == null) return NotFound();

            return View(volunteer);
        }

        // POST: Volunteers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Email,ApprovalStatus")] Volunteer volunteer)
        {
            if (id != volunteer.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(volunteer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VolunteerExists(volunteer.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(volunteer);
        }

        // GET: Volunteers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var volunteer = await _context.Volunteers
                .FirstOrDefaultAsync(m => m.Id == id);

            if (volunteer == null) return NotFound();

            return View(volunteer);
        }

        // POST: Volunteers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var volunteer = await _context.Volunteers.FindAsync(id);
            if (volunteer != null)
            {
                _context.Volunteers.Remove(volunteer);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Volunteers/Matches/5  (stub: show 10 most recent opportunities)
        public async Task<IActionResult> Matches(int id)
        {
            var volunteer = await _context.Volunteers.FindAsync(id);
            if (volunteer == null) return NotFound();

            var opportunities = await _context.Opportunities
                .OrderByDescending(o => o.CreatedDate)
                .Take(10)
                .ToListAsync();

            ViewBag.Volunteer = volunteer;
            return View(opportunities);
        }

        private bool VolunteerExists(int id) =>
            _context.Volunteers.Any(e => e.Id == id);
    }
}
