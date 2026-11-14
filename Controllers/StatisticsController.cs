using BeFit.Data;
using BeFit.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeFit.Controllers;

[Authorize]
public class StatisticsController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<IdentityUser> _users;

    public StatisticsController(ApplicationDbContext db, UserManager<IdentityUser> users)
    {
        _db = db;
        _users = users;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _users.GetUserId(User)!;
        var now = DateTime.Now;
        var cutoff = now.AddDays(-28);

        // statystyki tylko z ostatnich czterech tygodni
        var data = await _db.WorkoutExercises.AsNoTracking()
            .Where(x => x.UserId == userId && x.TrainingSession.UserId == userId &&
                        x.TrainingSession.StartedAt >= cutoff && x.TrainingSession.StartedAt <= now)
            .Select(x => new
            {
                ExerciseName = x.ExerciseType.Name,
                x.Sets,
                x.RepsPerSet,
                x.WeightKg
            })
            .ToListAsync();

        var viewModel = new ExerciseStatisticsViewModel
        {
            Rows = data.GroupBy(x => x.ExerciseName)
                .Select(g => new ExerciseStatisticsRow
                {
                    ExerciseName = g.Key,
                    TimesPerformed = g.Count(),
                    TotalRepetitions = g.Sum(x => x.Sets * x.RepsPerSet),
                    AverageWeightKg = Math.Round(g.Average(x => x.WeightKg), 2),
                    MaxWeightKg = g.Max(x => x.WeightKg)
                })
                .OrderBy(x => x.ExerciseName).ToList()
        };
        return View(viewModel);
    }
}
