using ChessManager.Areas.Identity.Data;
using ChessManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChessManager.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: UserController
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var usersWithRole = new List<UserWithRole>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                usersWithRole.Add(new UserWithRole
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    BirthDate = user.BirthDate,
                    Gender = user.Gender, // Pobranie płci z obiektu użytkownika
                    Role = roles.FirstOrDefault() ?? "No Role"
                });
            }

            return View(usersWithRole);
        }

        // GET: UserController/Create
        public IActionResult Create()
        {
            return View(new UserWithRole());
        }

        // POST: UserController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserWithRole model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    BirthDate = model.BirthDate,
                    Gender = model.Gender // Użycie Gender z modelu
                };

                var result = await _userManager.CreateAsync(user);

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.Role))
                    {
                        var roleExists = await _roleManager.RoleExistsAsync(model.Role);
                        if (roleExists)
                        {
                            await _userManager.AddToRoleAsync(user, model.Role);
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, $"The role '{model.Role}' does not exist.");
                            return View(model);
                        }
                    }

                    // Przekierowanie na widok Index
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }
    }
}
