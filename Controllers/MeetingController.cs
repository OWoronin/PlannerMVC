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
    public class MeetingController : Controller
    {
        private readonly PlannerContext _context;

        public MeetingController(PlannerContext context)
        {
            _context = context;
        }

        // GET: Meeting
        public async Task<IActionResult> Index()
        {
            var plannerContext = _context.Planners.Include(p => p.Days).ThenInclude(d => d.Meetings).ThenInclude(t => t.Priority).First();
            return View(plannerContext);
        }

        // GET: Meeting/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var meeting = await _context.Meetings
                .Include(m => m.Day)
                .Include(m => m.Priority)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (meeting == null)
            {
                return NotFound();
            }

            return View(meeting);
        }

        // GET: Meeting/Create
        public IActionResult Create(int? dayId)
        {
            int day = dayId ?? 0;

            var meeting = new Meeting
            {
                DayId = day
            };
            ViewData["DayId"] = new SelectList(_context.Days, "Id", "Name", day);
            ViewData["PriorityId"] = new SelectList(_context.Priorities, "Id", "Name");
            return View(meeting);
        }

        // POST: Meeting/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,PriorityId, Location,StartTime,EndTime,DayId")] Meeting meeting)
        {
            ModelState.Remove("CreatedDate");

            if (ModelState.IsValid)
            {
				meeting.CreatedDate = DateTime.Now;
				_context.Add(meeting);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DayId"] = new SelectList(_context.Days, "Id", "Name", meeting.DayId);
            ViewData["PriorityId"] = new SelectList(_context.Priorities, "Id", "Name", meeting.PriorityId);
            return View(meeting);
        }

        // GET: Meeting/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var meeting = await _context.Meetings.FindAsync(id);
            if (meeting == null)
            {
                return NotFound();
            }
            ViewData["DayId"] = new SelectList(_context.Days, "Id", "Name", meeting.DayId);
            ViewData["PriorityId"] = new SelectList(_context.Priorities, "Id", "Name", meeting.PriorityId);
            return View(meeting);
        }

        // POST: Meeting/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,PriorityId,CreatedDate,Location,StartTime,EndTime,DayId")] Meeting meeting)
        {
            if (id != meeting.Id)
            {
                return NotFound();
            }

			ModelState.Remove("CreatedDate");

			if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(meeting);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MeetingExists(meeting.Id))
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
            ViewData["DayId"] = new SelectList(_context.Days, "Id", "Name", meeting.DayId);
            ViewData["PriorityId"] = new SelectList(_context.Priorities, "Id", "Name", meeting.PriorityId);
            return View(meeting);
        }

        // GET: Meeting/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var meeting = await _context.Meetings
                .Include(m => m.Day)
                .Include(m => m.Priority)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (meeting == null)
            {
                return NotFound();
            }

            return View(meeting);
        }

        // POST: Meeting/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var meeting = await _context.Meetings.FindAsync(id);
            if (meeting != null)
            {
                _context.Meetings.Remove(meeting);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MeetingExists(int id)
        {
            return _context.Meetings.Any(e => e.Id == id);
        }
    }
}
