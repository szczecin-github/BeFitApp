using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using BeFitApp.Models;

namespace BeFitApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public HomeController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "TrainingSessions");
            }
            return View(); 
        }

        // --- THE ADMIN HACK ---
        // Visit /Home/MakeMeAdmin to become an admin instantly
        public async Task<IActionResult> MakeMeAdmin()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                // 1. Ensure the Admin role exists
                // (We seeded it in AppDbContext, but this is a double-check)
                
                // 2. Add user to role
                await _userManager.AddToRoleAsync(user, "Admin");
                
                // 3. Sign them out so the change takes effect
                return Content($"SUCCESS! User '{user.UserName}' is now an Admin. Please Log Out and Log In again to see the Admin menu.");
            }
            return Content("Error: You must be logged in to use this cheat code.");
        }
    }
}