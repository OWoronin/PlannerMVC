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


        //i have to copy this code from task controller ;_; 
        public async Task<IActionResult> Complete(int id)
        {
            var taskModel = await _context.Tasks.FindAsync(id);
            if (taskModel != null)
            {
                var taskStatus = await _context.Statuses.FindAsync(3);
                if (taskStatus != null)
                {
                    taskModel.Status = taskStatus;
                }
                else
                {
                    return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                }
            }
            else
            {
                return NotFound();
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
