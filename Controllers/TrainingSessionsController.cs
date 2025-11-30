using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using BeFitApp.Data;
using BeFitApp.Models;

namespace BeFitApp.Controllers
{
    [Authorize]
    public class TrainingSessionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public TrainingSessionsController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Index
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var sessions = await _context.TrainingSessions
                .Where(s => s.UserId == user.Id)
                .OrderByDescending(s => s.StartDateTime)
                .Include(s => s.Exercises)
                .ToListAsync();
            return View(sessions);
        }

        // GET: Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var session = await _context.TrainingSessions
                .Include(s => s.Exercises).ThenInclude(e => e.ExerciseType)
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == user.Id);

            if (session == null) return NotFound();

            // Populate list for the "Add Exercise" form on the details page
            ViewData["ExerciseTypeId"] = new SelectList(_context.ExerciseTypes, "Id", "Name");
            return View(session);
        }

        // GET: Create
        public IActionResult Create() => View();

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TrainingSession session)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            session.UserId = user.Id;
            ModelState.Remove("UserId");
            ModelState.Remove("User");

            // Validation: End time must be after Start time
            if (session.EndDateTime < session.StartDateTime)
            {
                ModelState.AddModelError("EndDateTime", "Data zakończenia musi być późniejsza niż rozpoczęcia.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(session);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = session.Id });
            }
            return View(session);
        }

        // GET: Edit (Required for CRUD)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var user = await _userManager.GetUserAsync(User);
            
            var session = await _context.TrainingSessions.FirstOrDefaultAsync(s => s.Id == id && s.UserId == user.Id);
            if (session == null) return NotFound();

            return View(session);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TrainingSession session)
        {
            if (id != session.Id) return NotFound();
            var user = await _userManager.GetUserAsync(User);

            // Ensure user owns this session before saving
            if (!_context.TrainingSessions.Any(s => s.Id == id && s.UserId == user.Id))
            {
                return NotFound();
            }

            session.UserId = user.Id; // Keep ownership
            ModelState.Remove("UserId");
            ModelState.Remove("User");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(session);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.TrainingSessions.Any(e => e.Id == session.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(session);
        }

        // GET: Delete (Required for CRUD)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var user = await _userManager.GetUserAsync(User);

            var session = await _context.TrainingSessions
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == user.Id);

            if (session == null) return NotFound();

            return View(session);
        }

        // POST: Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var session = await _context.TrainingSessions.FirstOrDefaultAsync(s => s.Id == id && s.UserId == user.Id);
            
            if (session != null)
            {
                _context.TrainingSessions.Remove(session);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Add Exercise
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddExercise(int TrainingSessionId, int ExerciseTypeId, double Weight, int Sets, int Repetitions)
        {
            var user = await _userManager.GetUserAsync(User);
            if (ExerciseTypeId == 0) return RedirectToAction(nameof(Details), new { id = TrainingSessionId });

            var session = await _context.TrainingSessions.FirstOrDefaultAsync(s => s.Id == TrainingSessionId && s.UserId == user.Id);
            
            if (session != null)
            {
                var exercise = new Exercise 
                { 
                    TrainingSessionId = TrainingSessionId, 
                    ExerciseTypeId = ExerciseTypeId, 
                    Weight = Weight,
                    Sets = Sets,
                    Repetitions = Repetitions
                };
                _context.Exercises.Add(exercise);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Details), new { id = TrainingSessionId });
        }
    }
}