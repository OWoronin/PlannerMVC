using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pz_Proj_11_12.Data;
using Pz_Proj_11_12.Models;
using Pz_Proj_11_12.Utils;


namespace Pz_Proj_11_12.Controllers
{
    public class MeetingController : Controller
    {
        private readonly PlannerContext _context;

        public MeetingController(PlannerContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            var plannerContext = _context.Planners.Include(p => p.Days).ThenInclude(d => d.Meetings).ThenInclude(t => t.Priority).First();
            return View(plannerContext);
        }

        // GET: Meeting/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            string referrer = Request.Headers.Referer.ToString();

            var isFromHome = RequestUtils.IsFromHomeController(referrer);

            if (isFromHome)
            {
                TempData["Back"] = "Home";
            }
            else
            {
                TempData["Back"] = "Meeting";
            }

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

      

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,PriorityId, Location,StartTime,EndTime,DayId")] Meeting meeting)
        {
            ModelState.Remove("CreatedDate");
            ModelState.Remove("Day");
            ModelState.Remove("Priority");

            if (meeting.EndTime <= meeting.StartTime)
            {
                ModelState.AddModelError("EndTime", "The end time cannot be earlier than the start time.");
            }


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

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,PriorityId,Location,StartTime,EndTime,DayId")] Meeting meeting)
        {
            if (id != meeting.Id)
            {
                return NotFound();
            }

			ModelState.Remove("CreatedDate");
            ModelState.Remove("Day");
            ModelState.Remove("Priority");

            if (meeting.EndTime <= meeting.StartTime)
            {
                ModelState.AddModelError("EndTime", "The end time cannot be earlier than the start time.");
            }



            if (ModelState.IsValid)
            {
                try
                {
                    meeting.CreatedDate = _context.Meetings.AsNoTracking().First(f=>f.Id == id).CreatedDate; 
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
