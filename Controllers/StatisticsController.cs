using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BeFitApp.Data;
using BeFitApp.Models;

namespace BeFitApp.Controllers
{
    [Authorize]
    public class StatisticsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public StatisticsController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var fourWeeksAgo = DateTime.Now.AddDays(-28);

            var stats = await _context.Exercises
                .Include(e => e.TrainingSession)
                .Include(e => e.ExerciseType)
                .Where(e => e.TrainingSession.UserId == user.Id)
                .Where(e => e.TrainingSession.StartDateTime >= fourWeeksAgo)
                .GroupBy(e => e.ExerciseType.Name)
                .Select(g => new ExerciseStatViewModel 
                { 
                    Name = g.Key, 
                    Count = g.Count(), 
                    TotalRepetitions = g.Sum(x => x.Sets * x.Repetitions),
                    MaxWeight = g.Max(x => x.Weight),
                    AvgWeight = g.Average(x => x.Weight)
                })
                .ToListAsync();

            return View(stats);
        }
    }

    public class ExerciseStatViewModel {
        public string? Name { get; set; }
        public int Count { get; set; }
        public int TotalRepetitions { get; set; }
        public double MaxWeight { get; set; }
        public double AvgWeight { get; set; }
    }
}