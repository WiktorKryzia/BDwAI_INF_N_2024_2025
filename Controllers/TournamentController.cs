using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ChessManager.Areas.Identity.Data;
using ChessManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace ChessManager.Controllers
{
    public class TournamentController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TournamentController(ApplicationDBContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [AllowAnonymous]
        // GET: Tournament
        public async Task<IActionResult> Index()
        {
            var model = await _context.Tournaments.Include(t => t.Arbiter).Include(t => t.Players).ToListAsync();
            return View(model);
        }

        // GET: Tournament/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tournament = await _context.Tournaments
                .Include(t => t.Arbiter)
                .Include(t => t.Players)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tournament == null)
            {
                return NotFound();
            }

            return View(tournament);
        }

        // GET: Tournament/Create
        public IActionResult Create()
        {
            SetArbitersList();

            return View();
        }

        // POST: Tournament/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Location,MaxPlayers,StartDate,EndDate,CreationDate,ArbiterId")] Tournament tournament)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tournament);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(tournament);
        }

        // GET: Tournament/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tournament = await _context.Tournaments.FindAsync(id);
            if (tournament == null)
            {
                return NotFound();
            }

            SetArbitersList();

            return View(tournament);
        }

        // POST: Tournament/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Location,MaxPlayers,StartDate,EndDate,CreationDate,ArbiterId")] Tournament tournament)
        {
            if (id != tournament.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tournament);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TournamentExists(tournament.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            SetArbitersList();

            return View(tournament);
        }

        // GET: Tournament/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tournament = await _context.Tournaments
                .Include(t => t.Arbiter)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tournament == null)
            {
                return NotFound();
            }

            return View(tournament);
        }

        // POST: Tournament/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tournament = await _context.Tournaments.FindAsync(id);
            if (tournament != null)
            {
                _context.Tournaments.Remove(tournament);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TournamentExists(int id)
        {
            return _context.Tournaments.Any(e => e.Id == id);
        }

        private void SetArbitersList()
        {
            var currentUserId = _userManager.GetUserId(User);

            var isAdmin = _context.UserRoles
                .Join(_context.Roles,
                      userRole => userRole.RoleId,
                      role => role.Id,
                      (userRole, role) => new { userRole.UserId, role.Name })
                .Any(ur => ur.UserId == currentUserId && ur.Name == "Admin");

            if (isAdmin)
            {
                var arbiters = _context.Users
                .Where(u => !_context.UserRoles
                .Join(_context.Roles,
                      userRole => userRole.RoleId,
                      role => role.Id,
                      (userRole, role) => new { userRole.UserId, role.Name })
                .Any(ur => ur.UserId == u.Id && ur.Name == "Player"))
                .Select(a => new
                {
                    ArbiterId = a.Id,
                    FullName = $"{a.FirstName} {a.LastName}"
                })
                .ToList();
                ViewData["ArbiterId"] = new SelectList(arbiters, "ArbiterId", "FullName");
            }
            else
            {
                ViewBag.ArbiterId = currentUserId;
            }
        }
    }
}
