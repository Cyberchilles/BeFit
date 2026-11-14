
using BeFit.Data;
using BeFit.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BeFit.Controllers
{
    [Authorize]
    public class TrainingSessionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TrainingSessionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // pobranie id aktualnego uzytkownika
        private string UserId =>
            User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        // lista moich treningow
        public async Task<IActionResult> Index()
        {
            var sessions = await _context.TrainingSessions
                .Where(x => x.UserId == UserId)
                .OrderByDescending(x => x.StartedAt)
                .ToListAsync();

            return View(sessions);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var session = await _context.TrainingSessions
                .Include(x => x.WorkoutExercises)
                .FirstOrDefaultAsync(x =>
                    x.Id == id && x.UserId == UserId);

            if (session == null) return NotFound();

            return View(session);
        }

        // nowa sesja
        public IActionResult Create()
        {
            return View(new TrainingSession
            {
                StartedAt = DateTime.Now,
                EndedAt = DateTime.Now.AddHours(1)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("StartedAt,EndedAt")] TrainingSession session)
        {
            // uzytkownika przypisujemy sami
            session.UserId = UserId;
            ModelState.Remove(nameof(TrainingSession.UserId));

            if (!ModelState.IsValid)
                return View(session);

            _context.TrainingSessions.Add(session);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // edycja tylko swojej sesji
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var session = await _context.TrainingSessions
                .FirstOrDefaultAsync(x =>
                    x.Id == id && x.UserId == UserId);

            if (session == null) return NotFound();

            return View(session);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("StartedAt,EndedAt")] TrainingSession input)
        {
            var session = await _context.TrainingSessions
                .FirstOrDefaultAsync(x =>
                    x.Id == id && x.UserId == UserId);

            if (session == null) return NotFound();

            if (!ModelState.IsValid)
                return View(input);

            session.StartedAt = input.StartedAt;
            session.EndedAt = input.EndedAt;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var session = await _context.TrainingSessions
                .FirstOrDefaultAsync(x =>
                    x.Id == id && x.UserId == UserId);

            if (session == null) return NotFound();

            return View(session);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var session = await _context.TrainingSessions
                .FirstOrDefaultAsync(x =>
                    x.Id == id && x.UserId == UserId);

            if (session == null) return NotFound();

            _context.TrainingSessions.Remove(session);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
