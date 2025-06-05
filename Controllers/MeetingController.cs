using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pz_Proj_11_12.Data;
using Pz_Proj_11_12.Models;
using Pz_Proj_11_12.Utils;
using Microsoft.AspNetCore.Authorization;
using Pz_Proj_11_12.ViewModels.Meeting;

namespace Pz_Proj_11_12.Controllers
{
    [Authorize]
    public class MeetingController : Controller
    {
        private readonly PlannerContext _context;
        private readonly IAuthorizationService _authorizationService;

        public MeetingController(PlannerContext context, IAuthorizationService authorizationService)
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

            var planner = _context.Planners
                .Include(p => p.Days)
                    .ThenInclude(d => d.Meetings)
                        .ThenInclude(t => t.Priority)
                .First(p => p.Id == id);

            var result = await _authorizationService.AuthorizeAsync(User, planner, HanderNames.Planner);
            if (!result.Succeeded) return RedirectToAction("Forbid", "Home");

            return View(planner);
        }

        public async Task<IActionResult> Details(int? id)
        {
            RequestUtils.GenerateBackData(Request.Headers.Referer.ToString(), TempData);
            if (id == null)
                return RedirectToAction("NotFoundPage", "Home");

            var meeting = await _context.Meetings
                .Include(m => m.Day)
                .Include(m => m.Priority)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (meeting == null)
                return RedirectToAction("NotFoundPage", "Home");

            var result = await _authorizationService.AuthorizeAsync(User, meeting, HanderNames.Meeting);
            if (!result.Succeeded)
                return RedirectToAction("Forbid", "Home");

            return View(meeting);
        }

        // GET: Meeting/Create
        public async Task<IActionResult> Create(int plannerId, int dayId)
        {
            RequestUtils.GenerateBackData(Request.Headers.Referer.ToString(), TempData);
            var planner = await _context.Planners
                .Include(p => p.Days)
                .FirstOrDefaultAsync(p => p.Id == plannerId);

            if (planner is null)
            {
                return RedirectToAction("NotFoundPage", "Home");
            }

            var resultPlanner = await _authorizationService.AuthorizeAsync(User, planner, HanderNames.Planner);
            if (!resultPlanner.Succeeded)
            {
                return RedirectToAction("Forbid", "Home");
            }

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

            var meeting = new CreateMeetingViewModel { DayId = day };

            ViewData["DayId"] = new SelectList(planner.Days, "Id", "Name", meeting?.DayId);
            ViewData["PriorityId"] = new SelectList(_context.Priorities, "Id", "Name");
            ViewData["plannerId"] = plannerId;

			RequestUtils.SetSessionFromReferrer(HttpContext);
			return View(meeting);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateMeetingViewModel meetingViewModel, int plannerId)
        {
            var planner = await _context.Planners
                .Include(p => p.Days)
                .FirstOrDefaultAsync(p => p.Id == plannerId);

            if (planner is null)
            {
                return RedirectToAction("NotFoundPage", "Home");
            }

            var plannerResult = await _authorizationService.AuthorizeAsync(User, planner, HanderNames.Planner);
            if (!plannerResult.Succeeded)
            {
                return RedirectToAction("Forbid", "Home");
            }

            var day = await _context.Days.FirstOrDefaultAsync(d => d.Id == meetingViewModel.DayId);

            if (day is null)
            {
                return RedirectToAction("NotFoundPage", "Home");
            }

            var dayResult = await _authorizationService.AuthorizeAsync(User, day, HanderNames.Day);
            if (!dayResult.Succeeded)
            {
                return RedirectToAction("Forbid", "Home");
            }

            if (meetingViewModel.EndTime <= meetingViewModel.StartTime)
            {
                ModelState.AddModelError("EndTime", "The end time cannot be earlier than the start time.");
            }

            if(meetingViewModel.IsWithReminder && meetingViewModel.ReminderName is null)
            {
				ModelState.AddModelError("ReminderName", "TODO: komunikat - nazwa rem");
			}

            if(meetingViewModel.IsWithReminder && meetingViewModel.HoursBefore is null)
            {
				ModelState.AddModelError("HoursBefore", "TODO: komunikat - nazwa hours");
			}

            // TODO: check for existing meeting in planner in day in time

            if (ModelState.IsValid)
            {
                var meeting = new Meeting()
                {
                    Name = meetingViewModel.Name,
                    StartTime = meetingViewModel.StartTime,
                    EndTime = meetingViewModel.EndTime,
                    Location = meetingViewModel.Location,
                    PriorityId = meetingViewModel.PriorityId,
                    DayId = meetingViewModel.DayId,
                    CreatedDate = DateTime.Now,
                    Description = meetingViewModel.Description
                };

                _context.Add(meeting);

                if (meetingViewModel.IsWithReminder)
                {
                    // whatever date
					var baseDate = new DateTime(2025, 6, 1);

                    // date + start meeting time
					var meetingDateTime = baseDate + meetingViewModel.StartTime.ToTimeSpan();

					// date + start meeting time - hours beefore
					var reminderDateTime = meetingDateTime.AddHours(-(double)meetingViewModel.HoursBefore);

					var dayOffset = (meetingDateTime.Date - reminderDateTime.Date).Days;

                    var min = planner.Days.Min(d => d.Id);

                    int reminderDayId = meetingViewModel.DayId - dayOffset;

					if (dayOffset != 0 && meetingViewModel.DayId == min)
                    {
                        reminderDayId = min;
                        reminderDateTime = meetingDateTime.AddHours((double)-1);

					}

					var reminder = new Reminder
					{
						Name = meetingViewModel.ReminderName,
						Description = $"Reminder to meeting: {meetingViewModel.Name}",
						CreatedDate = DateTime.Now,
						DayId = reminderDayId,
						ReminderTime = TimeOnly.FromDateTime(reminderDateTime)
					};

					_context.Reminders.Add(reminder);
				}

                await _context.SaveChangesAsync();
                var (controller, action, id) = RequestUtils.GetSessionValues(HttpContext);
                return RedirectToAction(action, controller, new { id });
            }
            ViewData["DayId"] = new SelectList(planner.Days, "Id", "Name", meetingViewModel.DayId);
            ViewData["PriorityId"] = new SelectList(_context.Priorities, "Id", "Name", meetingViewModel.PriorityId);
            ViewData["plannerId"] = plannerId;
            return View(meetingViewModel);
        }

        // GET: Meeting/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return RedirectToAction("NotFoundPage", "Home");

            var meeting = await _context.Meetings.FindAsync(id);
            if (meeting == null)
                return RedirectToAction("NotFoundPage", "Home");

            var result = await _authorizationService.AuthorizeAsync(User, meeting, HanderNames.Meeting);
            if (!result.Succeeded)
                return RedirectToAction("Forbid", "Home");

            var planner = await _context.Planners.Include(p => p.Days).Where(p => p.Days.Any(d => d.Id == meeting.DayId)).SingleOrDefaultAsync();

            ViewData["DayId"] = new SelectList(planner.Days, "Id", "Name", meeting.DayId);
            ViewData["PriorityId"] = new SelectList(_context.Priorities, "Id", "Name", meeting.PriorityId);
            ViewData["plannerId"] = planner.Id;
            return View(meeting);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Meeting meeting)
        {
            if (id != meeting.Id)
            {
                return RedirectToAction("NotFoundPage", "Home");
            }

            var existingMeeting = await _context.Meetings.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
            if (existingMeeting == null)
            {
                return RedirectToAction("NotFoundPage", "Home");
            }

            var result = await _authorizationService.AuthorizeAsync(User, existingMeeting, HanderNames.Meeting);
            if (!result.Succeeded)
            {
                return RedirectToAction("Forbid", "Home");
            }

            var planner = await _context.Planners.Include(p => p.Days).Where(p => p.Days.Any(d => d.Id == meeting.DayId)).SingleOrDefaultAsync();

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
                    meeting.CreatedDate = existingMeeting.CreatedDate;
                    _context.Update(meeting);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                     throw;
                }
                return RedirectToAction(nameof(Index), new { id = planner.Id });
            }
            ViewData["DayId"] = new SelectList(planner.Days, "Id", "Name", meeting.DayId);
            ViewData["PriorityId"] = new SelectList(_context.Priorities, "Id", "Name", meeting.PriorityId);
            ViewData["plannerId"] = planner.Id;
            return View(meeting);
        }

        // GET: Meeting/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return RedirectToAction("NotFoundPage", "Home");

            var meeting = await _context.Meetings
                .Include(m => m.Day)
                .Include(m => m.Priority)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (meeting == null)
                return RedirectToAction("NotFoundPage", "Home");

            var result = await _authorizationService.AuthorizeAsync(User, meeting, HanderNames.Meeting);
            if (!result.Succeeded)
                return RedirectToAction("Forbid", "Home");

            var planner = await _context.Planners.Include(p => p.Days).Where(p => p.Days.Any(d => d.Id == meeting.DayId)).SingleOrDefaultAsync();

            ViewData["plannerId"] = planner.Id;

            return View(meeting);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var meeting = await _context.Meetings.FindAsync(id);
            if (meeting != null)
            {
                var result = await _authorizationService.AuthorizeAsync(User, meeting, HanderNames.Meeting);
                if (!result.Succeeded)
                    return RedirectToAction("Forbid", "Home");

                _context.Meetings.Remove(meeting);
            }
            await _context.SaveChangesAsync();

            var planner = await _context.Planners.Include(p => p.Days).Where(p => p.Days.Any(d => d.Id == meeting.DayId)).SingleOrDefaultAsync();

            return RedirectToAction(nameof(Index), new { id = planner.Id });
        }
    }
}
