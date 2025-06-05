using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pz_Proj_11_12.Data;
using Pz_Proj_11_12.Models;
using Pz_Proj_11_12.Utils;

namespace Pz_Proj_11_12.Controllers
{
    [Authorize]
    public class ReminderController : Controller
    {
        private readonly PlannerContext _context;
        private readonly IAuthorizationService _authorizationService;

        public ReminderController(PlannerContext context, IAuthorizationService authorizationService)
        {
            _context = context;
            _authorizationService = authorizationService;
        }

        public async Task<IActionResult> Index(int? id)
        {
            if (id is null)
            {
                return RedirectToAction("Index", "Users");
            }

            var planner = await _context.Planners
                .Include(p => p.Days)
                    .ThenInclude(d => d.Reminders)
                .FirstOrDefaultAsync(p => p.Id == id);

            var result = await _authorizationService.AuthorizeAsync(User, planner, HanderNames.Planner);
            if (!result.Succeeded) return RedirectToAction("Forbid", "Home");

            return View(planner);
        }

        public async Task<IActionResult> Details(int? id)
        {
            RequestUtils.GenerateBackData(Request.Headers.Referer.ToString(), TempData);

            if (id == null)
                return RedirectToAction("NotFoundPage", "Home");

            var reminder = await _context.Reminders
                .Include(r => r.Day)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (reminder == null)
                return RedirectToAction("NotFoundPage", "Home");

            var result = await _authorizationService.AuthorizeAsync(User, reminder, HanderNames.Reminder);
            if (!result.Succeeded)
                return RedirectToAction("Forbid", "Home");

            return View(reminder);
        }

        public async Task<IActionResult> Create(int plannerId, int dayId)
        {
            var planner = await _context.Planners
                .Include(p => p.Days)
                .FirstOrDefaultAsync(p => p.Id == plannerId);

            if (planner == null)
            {
                return RedirectToAction("NotFoundPage", "Home");
            }

            var plannerResult = await _authorizationService.AuthorizeAsync(User, planner, HanderNames.Planner);
            if (!plannerResult.Succeeded)
                return RedirectToAction("Forbid", "Home");

            int day = 0;

            var dayFromDb = await _context.Days
                .FirstOrDefaultAsync(d => d.Id == dayId);

            if (dayFromDb is null)
            {
                return RedirectToAction("NotFoundPage", "Home");
            }

            var result = await _authorizationService.AuthorizeAsync(User, dayFromDb, HanderNames.Day);
            if (!result.Succeeded)
            {
                return RedirectToAction("Forbid", "Home");
            }

            day = dayId;

            ViewData["DayId"] = new SelectList(planner.Days, "Id", "Name", day);
            ViewData["plannerId"] = plannerId;

            RequestUtils.SetSessionFromReferrer(HttpContext);

            return View(new Reminder { DayId = day });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Reminder reminder, int plannerId)
        {
            var planner = await _context.Planners
                .Include(p => p.Days)
                .FirstOrDefaultAsync(p => p.Id == plannerId);

            if (planner == null)
            {
                return RedirectToAction("NotFoundPage", "Home");
            }

            var plannerResult = await _authorizationService.AuthorizeAsync(User, planner, HanderNames.Planner);
            if (!plannerResult.Succeeded)
                return RedirectToAction("Forbid", "Home");

            var day = await _context.Days.FirstOrDefaultAsync(d => d.Id == reminder.DayId);
            if (day == null)
            {
                return RedirectToAction("NotFoundPage", "Home");
            }

            var dayResult = await _authorizationService.AuthorizeAsync(User, day, HanderNames.Day);
            if (!dayResult.Succeeded)
                return RedirectToAction("Forbid", "Home");

            ModelState.Remove("CreatedDate");
            ModelState.Remove("Day");

            if (ModelState.IsValid)
            {
                reminder.CreatedDate = DateTime.Now;
                _context.Add(reminder);
                await _context.SaveChangesAsync();
                var (controller, action, id) = RequestUtils.GetSessionValues(HttpContext);
                return RedirectToAction(action, controller, new { id });
            }

            ViewData["DayId"] = new SelectList(planner.Days, "Id", "Name", reminder.DayId);
            ViewData["plannerId"] = planner.Id;
            return View(reminder);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return RedirectToAction("NotFoundPage", "Home");

            var reminder = await _context.Reminders.FindAsync(id);
            if (reminder == null)
                return RedirectToAction("NotFoundPage", "Home");

            var result = await _authorizationService.AuthorizeAsync(User, reminder, HanderNames.Reminder);
            if (!result.Succeeded)
                return RedirectToAction("Forbid", "Home");

            var planner = await _context.Planners.Include(p => p.Days).Where(p => p.Days.Any(d => d.Id == reminder.DayId)).SingleOrDefaultAsync();
            ViewData["DayId"] = new SelectList(planner.Days, "Id", "Name", reminder.DayId);
            ViewData["plannerId"] = planner.Id;
            return View(reminder);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Reminder reminder)
        {
            if (id != reminder.Id)
                return RedirectToAction("NotFoundPage", "Home");

            var existingReminder = await _context.Reminders.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
            if (existingReminder == null)
                return RedirectToAction("NotFoundPage", "Home");

            var result = await _authorizationService.AuthorizeAsync(User, existingReminder, HanderNames.Reminder);
            if (!result.Succeeded)
                return RedirectToAction("Forbid", "Home");

            var planner = await _context.Planners.Include(p => p.Days).Where(p => p.Days.Any(d => d.Id == reminder.DayId)).SingleOrDefaultAsync();

            ModelState.Remove("CreatedDate");
            ModelState.Remove("Day");

            if (ModelState.IsValid)
            {
                try
                {
                    reminder.CreatedDate = existingReminder.CreatedDate;
                    _context.Update(reminder);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return RedirectToAction(nameof(Index), new { id = planner.Id });
            }

            ViewData["DayId"] = new SelectList(planner.Days, "Id", "Name", reminder.DayId);
            ViewData["plannerId"] = planner.Id;
            return View(reminder);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return RedirectToAction("NotFoundPage", "Home");

            var reminder = await _context.Reminders
                .Include(r => r.Day)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reminder == null)
                return RedirectToAction("NotFoundPage", "Home");

            var result = await _authorizationService.AuthorizeAsync(User, reminder, HanderNames.Reminder);
            if (!result.Succeeded)
                return RedirectToAction("Forbid", "Home");

            var planner = await _context.Planners.Include(p => p.Days).Where(p => p.Days.Any(d => d.Id == reminder.DayId)).SingleOrDefaultAsync();

            ViewData["plannerId"] = planner.Id;

            return View(reminder);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reminder = await _context.Reminders.FindAsync(id);
            if (reminder != null)
            {
                var result = await _authorizationService.AuthorizeAsync(User, reminder, HanderNames.Reminder);
                if (!result.Succeeded)
                    return RedirectToAction("Forbid", "Home");

                _context.Reminders.Remove(reminder);
            }

            await _context.SaveChangesAsync();

            var planner = await _context.Planners.Include(p => p.Days).Where(p => p.Days.Any(d => d.Id == reminder.DayId)).SingleOrDefaultAsync();
            return RedirectToAction(nameof(Index), new { id = planner.Id });
        }
    }
}
