using Microsoft.AspNetCore.Mvc;
using Pz_Proj_11_12.Data;
using Pz_Proj_11_12.ViewModels.Search;

namespace Pz_Proj_11_12.Controllers
{
	public class SearchController : Controller
	{
        private readonly PlannerContext _context;
        private const int PageSize = 10;

        public SearchController(PlannerContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchQuery = "", string sortColumn = "Name", string sortDirection = "asc", int page = 1)
        {
            var tasks = _context.Tasks
                .Select(t => new SearchViewModel
                {
                    Id = t.Id,
                    Name = t.Name,
                    CreatedDate = t.CreatedDate,
                    Type = "Task"
                });

            var meetings = _context.Meetings
                .Select(m => new SearchViewModel
                {
                    Id = m.Id,
                    Name = m.Name,
                    CreatedDate = m.CreatedDate,
                    Type = "Meeting"
                });

            var reminders = _context.Reminders
                .Select(r => new SearchViewModel
                {
                    Id = r.Id,
                    Name = r.Name,
                    CreatedDate = r.CreatedDate,
                    Type = "Reminder"
                });

            var combined = tasks.Concat(meetings).Concat(reminders);

            if (!string.IsNullOrEmpty(searchQuery))
            {
                combined = combined.Where(i => i.Name.Contains(searchQuery));
            }

            combined = sortColumn switch
            {
                "CreatedDate" => sortDirection == "asc"
                    ? combined.OrderBy(i => i.CreatedDate)
                    : combined.OrderByDescending(i => i.CreatedDate),
                _ => sortDirection == "asc"
                    ? combined.OrderBy(i => i.Name)
                    : combined.OrderByDescending(i => i.Name)
            };

            var paginated = await Pagination<SearchViewModel>.Create(combined.AsQueryable(), page, PageSize);

            var viewModel = new SearchResultViewModel
            {
                SearchQuery = searchQuery,
                SortColumn = sortColumn,
                SortDirection = sortDirection,
                Results = paginated
            };

            return View(viewModel);
        }
    }
}

