using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pz_Proj_11_12.Data;
using Pz_Proj_11_12.Models;
using Pz_Proj_11_12.Utils;
using System.Web;

namespace Pz_Proj_11_12.Controllers
{
    public class TaskController : Controller
    {
        private readonly PlannerContext _context;

        public TaskController(PlannerContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {

            var plannerContext = _context.Planners.Include(p => p.Days).ThenInclude(d => d.Tasks).ThenInclude(t => t.Status)
                .Include(p => p.Days).ThenInclude(d => d.Tasks).ThenInclude(t => t.Priority)
                .Include(p => p.Days).ThenInclude(d => d.Tasks).ThenInclude(t => t.Difficulty).First();
            return View(plannerContext);
        }


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
                TempData["Back"] = "Task";
            } 

            if (id == null)
            {
                return NotFound();
            }

            var taskModel = await _context.Tasks
                .Include(t => t.Day)
                .Include(t => t.Difficulty)
                .Include(t => t.Priority)
                .Include(t => t.Status)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (taskModel == null)
            {
                return NotFound();
            }



            return View(taskModel);
        }


        public IActionResult Create(int? dayId)
        {
            int day = dayId ?? 0;

            var taskModel = new TaskModel
            {
                DayId = day
            };

            ViewData["DayId"] = new SelectList(_context.Days, "Id", "Name", day);
            ViewData["DifficultyId"] = new SelectList(_context.Difficulties, "Id", "Name");
            ViewData["PriorityId"] = new SelectList(_context.Priorities, "Id", "Name");
            ViewData["StatusId"] = new SelectList(_context.Statuses, "Id", "Name");

            return View(taskModel);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,DifficultyId,PriorityId,StatusId,DayId")] TaskModel taskModel)
        {
            ModelState.Remove("CreatedDate");
            ModelState.Remove("Day");
            ModelState.Remove("Status");
            ModelState.Remove("Priority");
            ModelState.Remove("Difficulty");

            if (ModelState.IsValid)
            {
                taskModel.CreatedDate = DateTime.Now;
                _context.Add(taskModel);
                await _context.SaveChangesAsync();
                string referrer = Request.Headers.Referer.ToString();

                return RedirectToAction(nameof(Index));
                
            }
            ViewData["DayId"] = new SelectList(_context.Days, "Id", "Name", taskModel.DayId);
            ViewData["DifficultyId"] = new SelectList(_context.Difficulties, "Id", "Name", taskModel.DifficultyId);
            ViewData["PriorityId"] = new SelectList(_context.Priorities, "Id", "Name", taskModel.PriorityId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "Id", "Name", taskModel.StatusId);
            return View(taskModel);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskModel = await _context.Tasks.FindAsync(id);
            if (taskModel == null)
            {
                return NotFound();
            }
            ViewData["DayId"] = new SelectList(_context.Days, "Id", "Name", taskModel.DayId);
            ViewData["DifficultyId"] = new SelectList(_context.Difficulties, "Id", "Name", taskModel.DifficultyId);
            ViewData["PriorityId"] = new SelectList(_context.Priorities, "Id", "Name", taskModel.PriorityId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "Id", "Name", taskModel.StatusId);
            return View(taskModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,DifficultyId,PriorityId,StatusId,DayId")] TaskModel taskModel)
        {

            if (id != taskModel.Id)
            {
                return NotFound();
            }

            ModelState.Remove("CreatedDate");
            ModelState.Remove("Day");
            ModelState.Remove("Status");
            ModelState.Remove("Priority");
            ModelState.Remove("Difficulty");



            if (ModelState.IsValid)
            {
                try
                {
                    taskModel.CreatedDate = _context.Tasks.AsNoTracking().First(f => f.Id == id).CreatedDate;
                    _context.Update(taskModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskModelExists(taskModel.Id))
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
            ViewData["DayId"] = new SelectList(_context.Days, "Id", "Name", taskModel.DayId);
            ViewData["DifficultyId"] = new SelectList(_context.Difficulties, "Id", "Name", taskModel.DifficultyId);
            ViewData["PriorityId"] = new SelectList(_context.Priorities, "Id", "Name", taskModel.PriorityId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "Id", "Name", taskModel.StatusId);
            return View(taskModel);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskModel = await _context.Tasks
                .Include(t => t.Day)
                .Include(t => t.Difficulty)
                .Include(t => t.Priority)
                .Include(t => t.Status)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (taskModel == null)
            {
                return NotFound();
            }

            return View(taskModel);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var taskModel = await _context.Tasks.FindAsync(id);
            if (taskModel != null)
            {
                _context.Tasks.Remove(taskModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TaskModelExists(int id)
        {
            return _context.Tasks.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Complete(int id)
        {
            string referrer = Request.Headers.Referer.ToString();

            var isFromHome = RequestUtils.IsFromHomeController(referrer);

            if (isFromHome)
            {
                TempData["Back"] = "Home";
            }
            else
            {
                TempData["Back"] = "Task";
            }

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

    }
}
