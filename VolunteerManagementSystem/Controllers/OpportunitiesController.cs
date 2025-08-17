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
    public class OpportunitiesController : Controller
    {
        private readonly AppDbContext _context;

        public OpportunitiesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Opportunities
        public async Task<IActionResult> Index(string? filter, string? center, string? q)
        {
            var query = _context.Opportunities.AsQueryable();

            // Most Recent (60 days)
            if (string.Equals(filter, "recent", StringComparison.OrdinalIgnoreCase))
            {
                var cutoff = DateTime.UtcNow.AddDays(-60);
                query = query.Where(o => o.CreatedDate >= cutoff);
            }

            // By Center (contains)
            if (!string.IsNullOrWhiteSpace(center))
            {
                query = query.Where(o => o.Center != null && o.Center.Contains(center));
            }

            // Search title/description
            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(o =>
                    o.Title.Contains(q) ||
                    (o.Description != null && o.Description.Contains(q)));
            }

            ViewBag.Filter = filter;
            ViewBag.Center = center;
            ViewBag.Query = q;

            var list = await query
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync();

            return View(list);
        }

        // GET: Opportunities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var opportunity = await _context.Opportunities
                .FirstOrDefaultAsync(m => m.Id == id);

            if (opportunity == null) return NotFound();

            return View(opportunity);
        }

        // GET: Opportunities/Create
        public IActionResult Create() => View();

        // POST: Opportunities/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Center,CreatedDate")] Opportunity opportunity)
        {
            if (ModelState.IsValid)
            {
                _context.Add(opportunity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(opportunity);
        }

        // GET: Opportunities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var opportunity = await _context.Opportunities.FindAsync(id);
            if (opportunity == null) return NotFound();

            return View(opportunity);
        }

        // POST: Opportunities/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Center,CreatedDate")] Opportunity opportunity)
        {
            if (id != opportunity.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(opportunity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OpportunityExists(opportunity.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(opportunity);
        }

        // GET: Opportunities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var opportunity = await _context.Opportunities
                .FirstOrDefaultAsync(m => m.Id == id);

            if (opportunity == null) return NotFound();

            return View(opportunity);
        }

        // POST: Opportunities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var opportunity = await _context.Opportunities.FindAsync(id);
            if (opportunity != null)
            {
                _context.Opportunities.Remove(opportunity);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Opportunities/Matches/5  (stub: show approved/pending volunteers)
        public async Task<IActionResult> Matches(int id)
        {
            var opportunity = await _context.Opportunities.FindAsync(id);
            if (opportunity == null) return NotFound();

            var volunteers = await _context.Volunteers
                .Where(v => v.ApprovalStatus == ApprovalStatus.Approved
                         || v.ApprovalStatus == ApprovalStatus.PendingApproval)
                .OrderBy(v => v.LastName).ThenBy(v => v.FirstName)
                .Take(10)
                .ToListAsync();

            ViewBag.Opportunity = opportunity;
            return View(volunteers);
        }

        private bool OpportunityExists(int id) =>
            _context.Opportunities.Any(e => e.Id == id);
    }
}
