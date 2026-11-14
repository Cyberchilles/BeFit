using BeFit.Data;
using BeFit.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BeFit.Controllers;

[Authorize]
public class WorkoutExercisesController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<IdentityUser> _users;

    public WorkoutExercisesController(ApplicationDbContext db, UserManager<IdentityUser> users)
    {
        _db = db;
        _users = users;
    }

    private string UserId => _users.GetUserId(User)!;

    private IQueryable<WorkoutExercise> OwnExercises() => _db.WorkoutExercises
        .Where(x => x.UserId == UserId && x.TrainingSession.UserId == UserId);

    private async Task FillLists(int? selectedType = null, int? selectedSession = null)
    {
        ViewBag.ExerciseTypes = (await _db.ExerciseTypes.AsNoTracking()
            .OrderBy(x => x.Name).ToListAsync())
            .Select(x => new SelectListItem(x.Name, x.Id.ToString(), x.Id == selectedType))
            .ToList();

        ViewBag.TrainingSessions = (await _db.TrainingSessions.AsNoTracking()
            .Where(x => x.UserId == UserId)
            .OrderByDescending(x => x.StartedAt).ToListAsync())
            .Select(x => new SelectListItem(
                $"{x.StartedAt:dd.MM.yyyy HH:mm} – {x.EndedAt:HH:mm}",
                x.Id.ToString(), x.Id == selectedSession))
            .ToList();
    }

    public async Task<IActionResult> Index() => View(await OwnExercises().AsNoTracking()
        .Include(x => x.ExerciseType)
        .Include(x => x.TrainingSession)
        .OrderByDescending(x => x.TrainingSession.StartedAt).ToListAsync());

    public async Task<IActionResult> Details(int id)
    {
        var item = await OwnExercises().AsNoTracking()
            .Include(x => x.ExerciseType).Include(x => x.TrainingSession)
            .FirstOrDefaultAsync(x => x.Id == id);
        return item == null ? NotFound() : View(item);
    }

    public async Task<IActionResult> Create(int? sessionId)
    {
        await FillLists(selectedSession: sessionId);
        return View(new WorkoutExercise { TrainingSessionId = sessionId ?? 0 });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("ExerciseTypeId,TrainingSessionId,WeightKg,Sets,RepsPerSet")] WorkoutExercise input)
    {
        ModelState.Remove(nameof(WorkoutExercise.UserId));
        await CheckReferences(input);
        if (!ModelState.IsValid)
        {
            await FillLists(input.ExerciseTypeId, input.TrainingSessionId);
            return View(input);
        }

        input.UserId = UserId;
        _db.WorkoutExercises.Add(input);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var item = await OwnExercises().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (item == null) return NotFound();
        await FillLists(item.ExerciseTypeId, item.TrainingSessionId);
        return View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id,
        [Bind("ExerciseTypeId,TrainingSessionId,WeightKg,Sets,RepsPerSet")] WorkoutExercise input)
    {
        var item = await OwnExercises().FirstOrDefaultAsync(x => x.Id == id);
        if (item == null) return NotFound();

        input.Id = id;
        ModelState.Remove(nameof(WorkoutExercise.UserId));
        await CheckReferences(input);
        if (!ModelState.IsValid)
        {
            await FillLists(input.ExerciseTypeId, input.TrainingSessionId);
            return View(input);
        }

        item.ExerciseTypeId = input.ExerciseTypeId;
        item.TrainingSessionId = input.TrainingSessionId;
        item.WeightKg = input.WeightKg;
        item.Sets = input.Sets;
        item.RepsPerSet = input.RepsPerSet;
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private async Task CheckReferences(WorkoutExercise input)
    {
        if (!await _db.ExerciseTypes.AnyAsync(x => x.Id == input.ExerciseTypeId))
            ModelState.AddModelError(nameof(input.ExerciseTypeId), "Wybierz istniejący typ ćwiczenia.");

        if (!await _db.TrainingSessions.AnyAsync(
            x => x.Id == input.TrainingSessionId && x.UserId == UserId))
            ModelState.AddModelError(nameof(input.TrainingSessionId), "Wybierz własną sesję treningową.");
    }

    public async Task<IActionResult> Delete(int id)
    {
        var item = await OwnExercises().AsNoTracking()
            .Include(x => x.ExerciseType).Include(x => x.TrainingSession)
            .FirstOrDefaultAsync(x => x.Id == id);
        return item == null ? NotFound() : View(item);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await OwnExercises().FirstOrDefaultAsync(x => x.Id == id);
        if (item == null) return NotFound();
        _db.WorkoutExercises.Remove(item);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
