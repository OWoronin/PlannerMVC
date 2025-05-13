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
    public class TaskController : Controller
    {
        private readonly PlannerContext _context;

        public TaskController(PlannerContext context)
        {
            _context = context;
        }

        // GET: Task
        public async Task<IActionResult> Index()
        {
            var plannerContext = _context.Tasks.Include(t => t.Day).Include(t => t.Difficulty).Include(t => t.Priority).Include(t => t.Status);
            return View(await plannerContext.ToListAsync());
        }

        // GET: Task/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: Task/Create
        public IActionResult Create()
        {
            ViewData["DayId"] = new SelectList(_context.Days, "Id", "Id");
            ViewData["DifficultyId"] = new SelectList(_context.Difficulties, "Id", "Id");
            ViewData["PriorityId"] = new SelectList(_context.Priorities, "Id", "Id");
            ViewData["StatusId"] = new SelectList(_context.Statuses, "Id", "Id");
            return View();
        }

        // POST: Task/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,DifficultyId,PriorityId,StatusId,CreatedDate,DayId")] TaskModel taskModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(taskModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DayId"] = new SelectList(_context.Days, "Id", "Id", taskModel.DayId);
            ViewData["DifficultyId"] = new SelectList(_context.Difficulties, "Id", "Id", taskModel.DifficultyId);
            ViewData["PriorityId"] = new SelectList(_context.Priorities, "Id", "Id", taskModel.PriorityId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "Id", "Id", taskModel.StatusId);
            return View(taskModel);
        }

        // GET: Task/Edit/5
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
            ViewData["DayId"] = new SelectList(_context.Days, "Id", "Id", taskModel.DayId);
            ViewData["DifficultyId"] = new SelectList(_context.Difficulties, "Id", "Id", taskModel.DifficultyId);
            ViewData["PriorityId"] = new SelectList(_context.Priorities, "Id", "Id", taskModel.PriorityId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "Id", "Id", taskModel.StatusId);
            return View(taskModel);
        }

        // POST: Task/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,DifficultyId,PriorityId,StatusId,CreatedDate,DayId")] TaskModel taskModel)
        {
            if (id != taskModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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
            ViewData["DayId"] = new SelectList(_context.Days, "Id", "Id", taskModel.DayId);
            ViewData["DifficultyId"] = new SelectList(_context.Difficulties, "Id", "Id", taskModel.DifficultyId);
            ViewData["PriorityId"] = new SelectList(_context.Priorities, "Id", "Id", taskModel.PriorityId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "Id", "Id", taskModel.StatusId);
            return View(taskModel);
        }

        // GET: Task/Delete/5
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

        // POST: Task/Delete/5
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
    }
}
