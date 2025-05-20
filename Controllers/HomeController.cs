using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pz_Proj_11_12.Data;
using Pz_Proj_11_12.Models;
using System.Diagnostics;

namespace Pz_Proj_11_12.Controllers
{
    public class HomeController : Controller
    {
        private readonly PlannerContext _context; 


        public HomeController(PlannerContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var planner = _context
                .Planners.Include(p => p.Days).ThenInclude(d => d.Tasks).ThenInclude(t => t.Status)
                .Include(p => p.Days).ThenInclude(d => d.Tasks).ThenInclude(t => t.Priority)
                .Include(p => p.Days).ThenInclude(d => d.Tasks).ThenInclude(t => t.Difficulty)
                .Include(d => d.Days).ThenInclude(d => d.Reminders)
                .Include(d => d.Days).ThenInclude(d => d.Meetings).ThenInclude(m => m.Priority)
                .First();
            return View(planner);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
