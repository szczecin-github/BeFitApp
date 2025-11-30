using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BeFitApp.Data;
using BeFitApp.Models;

namespace BeFitApp.Controllers
{
    [Authorize]
    public class ExerciseTypesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ExerciseTypesController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            return View(await _context.ExerciseTypes.ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(ExerciseType exerciseType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(exerciseType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(exerciseType);
        }

        public IActionResult RequestNew() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestNew(ExerciseRequest request)
        {
            var user = await _userManager.GetUserAsync(User);
            request.RequestedById = user.Id;
            request.IsApproved = false;
            
            _context.ExerciseRequests.Add(request);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "TrainingSessions");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageRequests()
        {
            var requests = await _context.ExerciseRequests
                .Include(r => r.RequestedBy)
                .Where(r => !r.IsApproved)
                .ToListAsync();
            return View(requests);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveRequest(int id)
        {
            var req = await _context.ExerciseRequests.FindAsync(id);
            if (req != null)
            {
                var newType = new ExerciseType { Name = req.Name, Description = req.Reason };
                _context.ExerciseTypes.Add(newType);
                _context.ExerciseRequests.Remove(req);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ManageRequests));
        }
    }
}