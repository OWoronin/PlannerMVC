using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pz_Proj_11_12.Data;
using Pz_Proj_11_12.Utils;
using Pz_Proj_11_12.ViewModels.Statistics;

namespace Pz_Proj_11_12.Controllers
{
    [Authorize]
    public class StatisticsController : Controller
    {
        private readonly PlannerContext _context;
        private readonly IAuthorizationService _authorizationService;

        public StatisticsController(PlannerContext context, IAuthorizationService authorizationService)
        {
            _context = context;
            _authorizationService = authorizationService;
        }

        public async Task<IActionResult>Index(int? id)
        {

            if(id is null)
            {
                return RedirectToAction("Index", "Users");
            }

            var planner = await _context.Planners.FirstOrDefaultAsync(p => p.Id == id);

            if(planner is null)
            {
                return RedirectToAction("BadRequestPage", "Home");
            }
            
            var result = await _authorizationService.AuthorizeAsync(User, planner, HanderNames.Planner);
            if (!result.Succeeded)
            {
                return RedirectToAction("Forbid", "Home");
            }

            var sVM = new StatisticsViewModel();

            var daysId = _context.Planners.Where(p=> p.Id == id).Include(p => p.Days).First().Days.Select(d => d.Id);

            sVM.amountOfTasks = _context.Tasks.Where(t => daysId.Contains(t.DayId)).Count();
            sVM.amountOfMeetings = _context.Meetings.Where(m => daysId.Contains(m.DayId)).Count();
            sVM.amountOfReminders = _context.Reminders.Where(r => daysId.Contains(r.DayId)).Count();
            sVM.amountOfRemindersBefore12 = _context.Reminders.Where(r => daysId.Contains(r.DayId) && r.ReminderTime.Hour <= 12).Count();
            sVM.amountOfRemindersAfter12 = _context.Reminders.Where(r => daysId.Contains(r.DayId) && r.ReminderTime.Hour > 12).Count();

            sVM.amountOfMeetingsBefore12 = _context.Meetings.Where(r => daysId.Contains(r.DayId) && r.StartTime.Hour <= 12).Count();
            sVM.amountOfMeetingsAfter12 = _context.Meetings.Where(r => daysId.Contains(r.DayId) && r.StartTime.Hour > 12).Count();

    
            var daysVM = new List<DayViewModel>(7); 
            foreach(var dayId in daysId)
            {
                var day = new DayViewModel
                {
                    Name = _context.Days.Where(d => d.Id == dayId).First().Name,
                    amountOfTasks = _context.Tasks.Where(t => t.DayId == dayId).Count(),
                    amountOfMeetings = _context.Meetings.Where(m => m.DayId == dayId).Count(),
                    amountOfReminders = _context.Reminders.Where(r => r.DayId == dayId).Count()
                };
                daysVM.Add(day);
            }

            sVM.Days = daysVM;

            var difficulties = _context.Difficulties.ToList();
            var diffsVM = new List<DifficultyViewModel>(difficulties.Count);
            foreach(var difficulty in difficulties)
            {
                var difficultyVM = new DifficultyViewModel
                {
                    Name = difficulty.Name,
                    amountOfTasks = _context.Tasks.Where(t => daysId.Contains(t.DayId) && t.DifficultyId == difficulty.Id).Count()
                };
                diffsVM.Add(difficultyVM);

            }
			sVM.Difficulties = diffsVM;

			var priorities = _context.Priorities.ToList();
			var proVM = new List<PriorityViewModel>(priorities.Count);
			foreach (var pro in priorities)
			{
                var prioVM = new PriorityViewModel
                {
                    Name = pro.Name,
                    amountOfTasks = _context.Tasks.Where(t => daysId.Contains(t.DayId) && t.PriorityId == pro.Id).Count(),
                    amountOfMeetings = _context.Meetings.Where(m => daysId.Contains(m.DayId) && m.PriorityId == pro.Id).Count()
                };
                proVM.Add(prioVM); 

			}
            sVM.Priorities = proVM;

			var statuses = _context.Statuses.ToList();
			var statusVM = new List<StatusViewModel>(statuses.Count);
			foreach (var st in statuses)
			{
                var stVM = new StatusViewModel
                {
                    Name = st.Name,
                    amountOfTasks = _context.Tasks.Where(s => daysId.Contains(s.DayId) && s.DifficultyId == st.Id).Count()
                };
                statusVM.Add(stVM);

			}
			sVM.Statuses = statusVM;

			var locations = _context.Meetings.Include(m => m.Day).Where(d => d.Day.PlannerId == id).Select(m => m.Location).ToHashSet();
            var locationsVM = new List<LocationViewModel>(locations.Count);
            foreach (var location in locations)
            {
                var locVM = new LocationViewModel
                {
                    Name = location,
                    amountOfMeetings = _context.Meetings.Where(l => l.Location == location).Count()
                };
                locationsVM.Add(locVM);
            }
            sVM.Locations = locationsVM;

            return View(sVM);
        }
    }
}
