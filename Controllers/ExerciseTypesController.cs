
using BeFit.Data;
using BeFit.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeFit.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ExerciseTypesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExerciseTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // lista cwiczen, widoczna dla kazdego
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var exercises = await _context.ExerciseTypes
                .OrderBy(e => e.Name)
                .ToListAsync();

            return View(exercises);
        }

        // szczegoly danego cwiczenia
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var exercise = await _context.ExerciseTypes
                .FirstOrDefaultAsync(e => e.Id == id);

            if (exercise == null)
                return NotFound();

            return View(exercise);
        }

        // formularz dodawania
        public IActionResult Create()
        {
            return View();
        }

        // zapis nowego typu cwiczenia
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Name")] ExerciseType exercise)
        {
            if (!ModelState.IsValid)
                return View(exercise);

            _context.ExerciseTypes.Add(exercise);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // pobranie cwiczenia do edycji
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var exercise = await _context.ExerciseTypes.FindAsync(id);

            if (exercise == null)
                return NotFound();

            return View(exercise);
        }

        // zapis zmian
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id, [Bind("Id,Name")] ExerciseType exercise)
        {
            if (id != exercise.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(exercise);

            var existing = await _context.ExerciseTypes.FindAsync(id);

            if (existing == null)
                return NotFound();

            existing.Name = exercise.Name;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // potwierdzenie usuniecia
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var exercise = await _context.ExerciseTypes
                .FirstOrDefaultAsync(e => e.Id == id);

            if (exercise == null)
                return NotFound();

            return View(exercise);
        }

        // usuwamy dopiero po potwierdzeniu
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var exercise = await _context.ExerciseTypes.FindAsync(id);

            if (exercise == null)
                return NotFound();

            // nie usuwamy typu, jesli byl juz uzywany
            var used = await _context.WorkoutExercises
                .AnyAsync(e => e.ExerciseTypeId == id);

            if (used)
            {
                ModelState.AddModelError("",
                    "Nie mozna usunac cwiczenia, ktore jest juz w treningach.");

                return View("Delete", exercise);
            }

            _context.ExerciseTypes.Remove(exercise);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
