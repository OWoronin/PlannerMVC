using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pz_Proj_11_12.Data;
using Pz_Proj_11_12.ViewModels.Statistics;

namespace Pz_Proj_11_12.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly PlannerContext _context;

        public StatisticsController(PlannerContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            StatisticsViewModel sVM = new StatisticsViewModel();

            var daysId = _context.Planners.Where(p=> p.Id == 1).Include(p => p.Days).First().Days.Select(d => d.Id);

            sVM.amountOfTasks = _context.Tasks.Where(t => daysId.Contains(t.DayId)).Count();
            sVM.amountOfMeetings = _context.Meetings.Where(m => daysId.Contains(m.DayId)).Count();
            sVM.amountOfReminders = _context.Reminders.Where(r => daysId.Contains(r.DayId)).Count();
            sVM.amountOfRemindersBefore12 = _context.Reminders.Where(r => daysId.Contains(r.DayId) && r.ReminderTime.Hour <= 12).Count();
            sVM.amountOfRemindersAfter12 = _context.Reminders.Where(r => daysId.Contains(r.DayId) && r.ReminderTime.Hour > 12).Count();

            sVM.amountOfMeetingsBefore12 = _context.Meetings.Where(r => daysId.Contains(r.DayId) && r.StartTime.Hour <= 12).Count();
            sVM.amountOfMeetingsAfter12 = _context.Meetings.Where(r => daysId.Contains(r.DayId) && r.StartTime.Hour > 12).Count();

    
            var daysVM = new List<DayViewModel>(7); 
            foreach(var id in daysId)
            {
                var day = new DayViewModel(); 
                day.Name = _context.Days.Where(d=>d.Id == id).First().Name;
                day.amountOfTasks = _context.Tasks.Where(t => t.DayId == id).Count();
                day.amountOfMeetings = _context.Meetings.Where(m => m.DayId == id).Count();
                day.amountOfReminders = _context.Reminders.Where(r => r.DayId == id).Count();
                daysVM.Add(day);
            }

            sVM.Days = daysVM;

            var difficulties = _context.Difficulties.ToList();
            var diffsVM = new List<DifficultyViewModel>(difficulties.Count());
            foreach(var difficulty in difficulties)
            {
                var difficultyVM = new DifficultyViewModel();
                difficultyVM.Name = difficulty.Name;
                difficultyVM.amountOfTasks = _context.Tasks.Where(t => daysId.Contains(t.DayId) && t.DifficultyId == difficulty.Id).Count();
                diffsVM.Add(difficultyVM);

            }
			sVM.Difficulties = diffsVM;

			var priorities = _context.Priorities.ToList();
			var proVM = new List<PriorityViewModel>(priorities.Count());
			foreach (var pro in priorities)
			{
				var prioVM = new PriorityViewModel();
				prioVM.Name = pro.Name;
				prioVM.amountOfTasks = _context.Tasks.Where(t => daysId.Contains(t.DayId) && t.PriorityId == pro.Id).Count();
				prioVM.amountOfMeetings = _context.Meetings.Where(m => daysId.Contains(m.DayId) && m.PriorityId == pro.Id).Count();
                proVM.Add(prioVM); 

			}
            sVM.Priorities = proVM;

			var statuses = _context.Statuses.ToList();
			var statusVM = new List<StatusViewModel>(statuses.Count());
			foreach (var st in statuses)
			{
				var stVM = new StatusViewModel();
				stVM.Name = st.Name;
				stVM.amountOfTasks = _context.Tasks.Where(s => daysId.Contains(s.DayId) && s.DifficultyId == st.Id).Count();
				statusVM.Add(stVM);

			}
			sVM.Statuses = statusVM;

			var locations = _context.Meetings.Select(m => m.Location).ToHashSet();
            var locationsVM = new List<LocationViewModel>(locations.Count());
            foreach (var location in locations)
            {
                var locVM = new LocationViewModel();
                locVM.Name = location; 
                locVM.amountOfMeetings = _context.Meetings.Where(l=>l.Location == location).Count();
               locationsVM.Add(locVM);
            }
            sVM.Locations = locationsVM;

            return View(sVM);
        }
    }
}
