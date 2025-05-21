using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pz_Proj_11_12.Data;
using Pz_Proj_11_12.Models;

namespace Pz_Proj_11_12.Controllers
{
    public class ReminderController : Controller
    {
        private readonly PlannerContext _context;

        public ReminderController(PlannerContext context)
        {
            _context = context;
        }

        // GET: Reminder
        public async Task<IActionResult> Index()
        {
            var plannerContext = _context.Planners.Include(p => p.Days).ThenInclude(d => d.Reminders).First();
            return View(plannerContext);
        }

        // GET: Reminder/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)			
			{
                return NotFound();
            }

            var reminder = await _context.Reminders
                .Include(r => r.Day)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reminder == null)
            {
                return NotFound();
            }

            return View(reminder);
        }

        // GET: Reminder/Create
        public IActionResult Create(int? dayId)
        {
            int day = dayId ?? 0;

            var reminder = new Reminder
            {
                DayId = day
            }; 

            ViewData["DayId"] = new SelectList(_context.Days, "Id", "Name", day);
            return View(reminder);
        }

        // POST: Reminder/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,ReminderTime,DayId")] Reminder reminder)
        {
            ModelState.Remove("CreatedDate");

            if (ModelState.IsValid)
            {
                reminder.CreatedDate = DateTime.Now;
                _context.Add(reminder);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DayId"] = new SelectList(_context.Days, "Id", "Name", reminder.DayId);
            return View(reminder);
        }

        // GET: Reminder/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reminder = await _context.Reminders.FindAsync(id);
            if (reminder == null)
            {
                return NotFound();
            }
            ViewData["DayId"] = new SelectList(_context.Days, "Id", "Name", reminder.DayId);
            return View(reminder);
        }

        // POST: Reminder/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,ReminderTime,DayId")] Reminder reminder)
        {
            if (id != reminder.Id)
            {
                return NotFound();
            }

            ModelState.Remove("CreatedDate");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reminder);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReminderExists(reminder.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DayId"] = new SelectList(_context.Days, "Id", "Name", reminder.DayId);
            return View(reminder);
        }

        // GET: Reminder/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reminder = await _context.Reminders
                .Include(r => r.Day)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reminder == null)
            {
                return NotFound();
            }

            return View(reminder);
        }

        // POST: Reminder/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reminder = await _context.Reminders.FindAsync(id);
            if (reminder != null)
            {
                _context.Reminders.Remove(reminder);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReminderExists(int id)
        {
            return _context.Reminders.Any(e => e.Id == id);
        }
    }
}
