using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pz_Proj_11_12.Data;
using Pz_Proj_11_12.ViewModels;

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
            //before and after 12 
            sVM.amountOfRemindersBefore12 = _context.Reminders.Where(r => daysId.Contains(r.DayId) && r.ReminderTime.Hour <= 12).Count();
            sVM.amountOfRemindersAfter12 = _context.Reminders.Where(r => daysId.Contains(r.DayId) && r.ReminderTime.Hour > 12).Count();

            sVM.amountOfMeetingsBefore12 = _context.Meetings.Where(r => daysId.Contains(r.DayId) && r.StartTime.Hour <= 12).Count();
            sVM.amountOfMeetingsAfter12 = _context.Meetings.Where(r => daysId.Contains(r.DayId) && r.StartTime.Hour > 12).Count();

            //by days 
            var daysVM = new List<DayViewModel>(7); 
            foreach(var id in daysId)
            {
                var day = new DayViewModel(); 
                day.amountOfTasks = _context.Tasks.Where(t => t.DayId == id).Count();
                day.amountOfMeetings = _context.Meetings.Where(m => m.DayId == id).Count();
                day.amountOfReminders = _context.Reminders.Where(r => r.DayId == id).Count();
                daysVM.Add(day);
            }

            var difficulties = _context.Difficulties.ToList();
            var diffsVM = new List<DifficultyViewModel>(difficulties.Count());
            foreach(var difficulty in difficulties)
            {
                var difficultyVM = new DifficultyViewModel();
                difficultyVM.Name = difficulty.Name;
                difficultyVM.amountOfTasks = _context.Tasks.Where(t => daysId.Contains(t.DayId) && t.DifficultyId == difficulty.Id).Count();

            }
            //
            _context.Meetings.Select(m => m.Location).ToHashSet();



            return View();
        }
    }
}
