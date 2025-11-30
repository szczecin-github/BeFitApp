using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BeFitApp.Data;
using BeFitApp.Models;

namespace BeFitApp.Controllers
{
    [Authorize]
    public class FoodEntriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public FoodEntriesController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var entries = await _context.FoodEntries
                .Where(f => f.UserId == user.Id)
                .OrderByDescending(f => f.Date)
                .ToListAsync();
            return View(entries);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FoodEntry entry)
        {
            var user = await _userManager.GetUserAsync(User);
            entry.UserId = user.Id;

            ModelState.Remove("UserId"); 
            ModelState.Remove("User");

            if (ModelState.IsValid)
            {
                _context.Add(entry);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(entry);
        }
    }
}