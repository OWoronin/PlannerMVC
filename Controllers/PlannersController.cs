using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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
    public class PlannersController : Controller
    {
        private readonly PlannerContext _context;

        public PlannersController(PlannerContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Details(int? id)
        {

            if (id == null)
                return RedirectToAction("NotFoundPage", "Home");

            var planner = await _context.Planners
                .Include(p => p.Days).ThenInclude(d => d.Tasks).ThenInclude(t => t.Status)
                .Include(p => p.Days).ThenInclude(d => d.Tasks).ThenInclude(t => t.Priority)
                .Include(p => p.Days).ThenInclude(d => d.Tasks).ThenInclude(t => t.Difficulty)
                .Include(p => p.Days).ThenInclude(d => d.Reminders)
                .Include(p => p.Days).ThenInclude(d => d.Meetings).ThenInclude(m => m.Priority)
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (planner == null)
                return RedirectToAction("NotFoundPage", "Home");

            HttpContext.Session.SetInt32("plannerId", id.Value);

            return View(planner);
        }

        public IActionResult Create()
        {
            ViewData["UserId"] = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Planner planner)
        {

            ModelState.Remove("User");

            if (ModelState.IsValid)
            {
                _context.Add(planner);
                await _context.SaveChangesAsync();

                var days = new List<Day>();

                string[] dayNames = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"];
                for (int i = 0; i < 7; i++)
                {
                    var day = new Day()
                    {
                        Name = dayNames[i],
                        PlannerId = planner.Id,
                    };

                    days.Add(day);
                }

                _context.Days.AddRange(days);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Details), new {id = planner.Id});
            }
            return View(planner);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return RedirectToAction("NotFoundPage", "Home");

            var planner = await _context.Planners.FindAsync(id);
            if (planner == null)
                return RedirectToAction("NotFoundPage", "Home");

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (planner.UserId != userId)
            {
                return RedirectToAction("Index", "Users");
            }

            ViewData["UserId"] = userId;
            return View(planner);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Planner planner)
        {
           
            if (id != planner.Id)
                return RedirectToAction("NotFoundPage", "Home");
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (planner.UserId != userId)
            {
                return RedirectToAction("Index", "Users");
            }

            ModelState.Remove("User");

            if (ModelState.IsValid)
            {
                _context.Update(planner);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Users");
            }
            ViewData["UserId"] = userId;
            return View(planner);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            
            if (id == null)
                return RedirectToAction("NotFoundPage", "Home");

            var planner = await _context.Planners
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (planner == null)
                return RedirectToAction("NotFoundPage", "Home");
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (planner.UserId != userId)
            {
                return RedirectToAction("Index", "Users");
            }

            return View(planner);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var planner = await _context.Planners.FindAsync(id);
            if (planner != null)
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (planner.UserId != userId)
                {
                    return RedirectToAction("Index", "Users");
                }
                _context.Planners.Remove(planner);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Users");
        }
    }
}
