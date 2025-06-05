using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pz_Proj_11_12.Data;
using Pz_Proj_11_12.Models;
using Pz_Proj_11_12.Utils;
using Microsoft.AspNetCore.Authorization;

namespace Pz_Proj_11_12.Controllers
{
    [Authorize]
    public class TaskController : Controller
    {
        private readonly PlannerContext _context;
        private readonly IAuthorizationService _authorizationService;

        public TaskController(PlannerContext context, IAuthorizationService authorizationService)
        {
            _context = context;
            _authorizationService = authorizationService;
        }

        public async Task<IActionResult> Index(int? id)
        {
            if(id is null)
            {
                return RedirectToAction("Index", "Users");
            }

            var planner = _context.Planners.Include(p => p.Days).ThenInclude(d => d.Tasks).ThenInclude(t => t.Status)
                .Include(p => p.Days).ThenInclude(d => d.Tasks).ThenInclude(t => t.Priority)
                .Include(p => p.Days).ThenInclude(d => d.Tasks).ThenInclude(t => t.Difficulty).First(p => p.Id == id);

            var result = await _authorizationService.AuthorizeAsync(User, planner, HanderNames.Planner);
            if (!result.Succeeded) return RedirectToAction("Forbid", "Home");

            return View(planner);
        }

        public async Task<IActionResult> Details(int? id)
        {
            RequestUtils.GenerateBackData(Request.Headers.Referer.ToString(), TempData);

            if (id == null)
            {
                return RedirectToAction("NotFoundPage", "Home");
            }

            var taskModel = await _context.Tasks
                .Include(t => t.Day)
                .Include(t => t.Difficulty)
                .Include(t => t.Priority)
                .Include(t => t.Status)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (taskModel == null)
            {
                return RedirectToAction("NotFoundPage", "Home");
            }

            var result = await _authorizationService.AuthorizeAsync(User, taskModel, HanderNames.Task);
            if (!result.Succeeded) return RedirectToAction("Forbid", "Home");

            return View(taskModel);
        }


        // GET: Task/Create
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
            

            var taskModel = new TaskModel
            {
                DayId = day
            };

            RequestUtils.SetSessionFromReferrer(HttpContext);

            ViewData["DayId"] = new SelectList(planner.Days, "Id", "Name", taskModel?.DayId);
            ViewData["DifficultyId"] = new SelectList(_context.Difficulties, "Id", "Name", taskModel?.DifficultyId);
            ViewData["PriorityId"] = new SelectList(_context.Priorities, "Id", "Name", taskModel?.PriorityId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "Id", "Name", taskModel?.StatusId);
            ViewData["plannerId"] = plannerId;


            return View(taskModel);
        }

        // POST: Task/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskModel taskModel, int plannerId)
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

            var day = await _context.Days.FirstOrDefaultAsync(d => d.Id == taskModel.DayId);

            if (day is null)
            {
                return RedirectToAction("NotFoundPage", "Home");
            }

            var dayResult = await _authorizationService.AuthorizeAsync(User, day, HanderNames.Day);
            if (!dayResult.Succeeded)
            {
                return RedirectToAction("Forbid", "Home");
            }

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

                var (controller, action, id) = RequestUtils.GetSessionValues(HttpContext);

                return RedirectToAction(action, controller, new { id });
            }

            ViewData["DayId"] = new SelectList(planner.Days, "Id", "Name", taskModel?.DayId);
            ViewData["DifficultyId"] = new SelectList(_context.Difficulties, "Id", "Name", taskModel?.DifficultyId);
            ViewData["PriorityId"] = new SelectList(_context.Priorities, "Id", "Name", taskModel?.PriorityId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "Id", "Name", taskModel?.StatusId);
            ViewData["plannerId"] = plannerId;
            return View(taskModel);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("NotFoundPage", "Home");
            }

            var taskModel = await _context.Tasks.FindAsync(id);
            if (taskModel == null)
            {
                return RedirectToAction("NotFoundPage", "Home");
            }

            var taskResult = await _authorizationService.AuthorizeAsync(User, taskModel, HanderNames.Task);

            if (!taskResult.Succeeded) return RedirectToAction("Forbid", "Home");

            var planner = await _context.Planners.Include(p => p.Days).Where(p => p.Days.Any(d => d.Id == taskModel.DayId)).SingleOrDefaultAsync();

            ViewData["DayId"] = new SelectList(planner.Days, "Id", "Name", taskModel.DayId);
            ViewData["DifficultyId"] = new SelectList(_context.Difficulties, "Id", "Name", taskModel.DifficultyId);
            ViewData["PriorityId"] = new SelectList(_context.Priorities, "Id", "Name", taskModel.PriorityId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "Id", "Name", taskModel.StatusId);
            ViewData["plannerId"] = planner.Id;

            return View(taskModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TaskModel taskModel)
        {
            if (id != taskModel.Id)
            {
                return RedirectToAction("NotFoundPage", "Home");
            }

            var existingTask = await _context.Tasks.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
            if (existingTask == null)
            {
                return RedirectToAction("NotFoundPage", "Home");
            }

            var taskResult = await _authorizationService.AuthorizeAsync(User, taskModel, HanderNames.Task);

            if (!taskResult.Succeeded) return RedirectToAction("Forbid", "Home");

            var planner = await _context.Planners.Include(p => p.Days).Where(p=>p.Days.Any(d=> d.Id == taskModel.DayId)).SingleOrDefaultAsync();

            ModelState.Remove("CreatedDate");
            ModelState.Remove("Day");
            ModelState.Remove("Status");
            ModelState.Remove("Priority");
            ModelState.Remove("Difficulty");

            if (ModelState.IsValid)
            {
                try
                {
                    taskModel.CreatedDate = existingTask.CreatedDate;
                    _context.Update(taskModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }

                return RedirectToAction(nameof(Index), new { id = planner.Id });
            }
            ViewData["DayId"] = new SelectList(planner.Days, "Id", "Name", taskModel.DayId);
            ViewData["DifficultyId"] = new SelectList(_context.Difficulties, "Id", "Name", taskModel.DifficultyId);
            ViewData["PriorityId"] = new SelectList(_context.Priorities, "Id", "Name", taskModel.PriorityId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "Id", "Name", taskModel.StatusId);
            ViewData["plannerId"] = planner.Id;
            return View(taskModel);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("NotFoundPage", "Home");
            }

            var taskModel = await _context.Tasks
                .Include(t => t.Day)
                .Include(t => t.Difficulty)
                .Include(t => t.Priority)
                .Include(t => t.Status)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (taskModel == null)
            {
                return RedirectToAction("NotFoundPage", "Home");
            }

            var taskResult = await _authorizationService.AuthorizeAsync(User, taskModel, HanderNames.Task);

            if (!taskResult.Succeeded) return RedirectToAction("Forbid", "Home");

            var planner = await _context.Planners.Include(p => p.Days).Where(p => p.Days.Any(d => d.Id == taskModel.DayId)).SingleOrDefaultAsync();

            ViewData["plannerId"] = planner.Id;

            return View(taskModel);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var taskModel = await _context.Tasks.FindAsync(id);
            if (taskModel != null)
            {
                var taskResult = await _authorizationService.AuthorizeAsync(User, taskModel, HanderNames.Task);

                if (!taskResult.Succeeded) return RedirectToAction("Forbid", "Home");
                _context.Tasks.Remove(taskModel);
            }

            await _context.SaveChangesAsync();

            var planner = await _context.Planners.Include(p => p.Days).Where(p => p.Days.Any(d => d.Id == taskModel.DayId)).SingleOrDefaultAsync();
            return RedirectToAction(nameof(Index), new {id = planner.Id});
        }

        public async Task<IActionResult> Complete(int id)
        {
            var taskModel = await _context.Tasks.FindAsync(id);
            if (taskModel is null)
            {
                return RedirectToAction("BadRequestPage", "Home");
            }

            var taskResult = await _authorizationService.AuthorizeAsync(User, taskModel, HanderNames.Task);

            if (!taskResult.Succeeded)
            {
                return RedirectToAction("NotFoundPage", "Home");
            }
            else
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

            var referer = Request.Headers.Referer.ToString();

            var splitted = referer.Split("/", StringSplitOptions.RemoveEmptyEntries);

			await _context.SaveChangesAsync();
            var planner = await _context.Planners.Include(p => p.Days).Where(p => p.Days.Any(d => d.Id == taskModel.DayId)).SingleOrDefaultAsync();
            return RedirectToAction(splitted[^2], splitted[^3], new { id = planner.Id });
        }

    }
}
