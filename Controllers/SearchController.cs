using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pz_Proj_11_12.Data;
using Pz_Proj_11_12.Utils;
using Pz_Proj_11_12.ViewModels.Search;

namespace Pz_Proj_11_12.Controllers
{
	[Authorize]
	public class SearchController : Controller
	{
		private readonly PlannerContext _context;
		private readonly IAuthorizationService _authorizationService;
		private const int PageSize = 10;

		public SearchController(PlannerContext context, IAuthorizationService authorizationService)
		{
			_context = context;
			_authorizationService = authorizationService;
		}

		public async Task<IActionResult> Index(
			int id, string searchQuery = "", string sortColumn = "Name", string sortDirection = "asc", int page = 1,
			int FilterType = 0, // 0-all, 1-task, 2-meeting, 3-reminder
			int? TaskStatusId = null, int? TaskPriorityId = null, int? TaskDifficultyId = null,
			int? MeetingPriorityId = null, string? MeetingLocation = null
)
		{
			var planner = await _context.Planners.Include(p => p.Days).FirstOrDefaultAsync(p => p.Id == id);
			if (planner is null) return RedirectToAction("BadRequestPage", "Home");

			var resultPlanner = await _authorizationService.AuthorizeAsync(User, planner, HanderNames.Planner);
			if (!resultPlanner.Succeeded) return RedirectToAction("Forbid", "Home");

			IQueryable<SearchViewModel> combined = Enumerable.Empty<SearchViewModel>().AsQueryable();


			// if combined in ifs throws some exception, it cannot translate to sql
			var tasksResults = Enumerable.Empty<SearchViewModel>().AsQueryable();
			var meetingsResults = Enumerable.Empty<SearchViewModel>().AsQueryable();
			var remindersResults = Enumerable.Empty<SearchViewModel>().AsQueryable();

			if (FilterType == 1 || FilterType == 0)
			{
				var tasks = _context.Tasks.Include(t => t.Day).Where(t => t.Day.PlannerId == id);
				if (TaskStatusId.HasValue) tasks = tasks.Where(t => t.StatusId == TaskStatusId);
				if (TaskPriorityId.HasValue) tasks = tasks.Where(t => t.PriorityId == TaskPriorityId);
				if (TaskDifficultyId.HasValue) tasks = tasks.Where(t => t.DifficultyId == TaskDifficultyId);

				tasksResults = tasks.Select(t => new SearchViewModel { Id = t.Id, Name = t.Name, CreatedDate = t.CreatedDate, Type = "Task" });
			}
			if (FilterType == 2 || FilterType == 0)
			{
				var meetings = _context.Meetings.Include(m => m.Day).Where(m => m.Day.PlannerId == id);
				if (MeetingPriorityId.HasValue) meetings = meetings.Where(m => m.PriorityId == MeetingPriorityId);
				if (!string.IsNullOrWhiteSpace(MeetingLocation)) meetings = meetings.Where(m => m.Location.Contains(MeetingLocation));

				meetingsResults = meetings.Select(m => new SearchViewModel { Id = m.Id, Name = m.Name, CreatedDate = m.CreatedDate, Type = "Meeting" });
			}
			if (FilterType == 3 || FilterType == 0)
			{
				var reminders = _context.Reminders.Include(r => r.Day).Where(r => r.Day.PlannerId == id);
				remindersResults = reminders.Select(r => new SearchViewModel { Id = r.Id, Name = r.Name, CreatedDate = r.CreatedDate, Type = "Reminder" });
			}

			combined = tasksResults.ToList()
				.Concat(meetingsResults.ToList())
				.Concat(remindersResults.ToList())
				.AsQueryable();


			if (!string.IsNullOrEmpty(searchQuery))
				combined = combined.Where(i => i.Name.Contains(searchQuery, StringComparison.CurrentCultureIgnoreCase));

			combined = sortColumn switch
			{
				"CreatedDate" => sortDirection == "asc" ? combined.OrderBy(i => i.CreatedDate) : combined.OrderByDescending(i => i.CreatedDate),
				_ => sortDirection == "asc" ? combined.OrderBy(i => i.Name) : combined.OrderByDescending(i => i.Name)
			};

			var paginated = Pagination<SearchViewModel>.Create(combined, page, PageSize);


			var viewModel = new SearchResultViewModel
			{
				SearchQuery = searchQuery,
				SortColumn = sortColumn,
				SortDirection = sortDirection,
				Results = paginated,

				FilterType = FilterType,

				TaskStatusId = TaskStatusId,
				TaskPriorityId = TaskPriorityId,
				TaskDifficultyId = TaskDifficultyId,
				MeetingPriorityId = MeetingPriorityId,
				MeetingLocation = MeetingLocation,

				Statuses = await _context.Statuses.ToListAsync(),
				Priorities = await _context.Priorities.ToListAsync(),
				Difficulties = await _context.Difficulties.ToListAsync()
			};

			return View(viewModel);
		}


	}
}

