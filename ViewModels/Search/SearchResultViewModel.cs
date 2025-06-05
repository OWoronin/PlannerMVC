using Pz_Proj_11_12.Models.LookupTables;

namespace Pz_Proj_11_12.ViewModels.Search
{
	public class SearchResultViewModel
	{
		public string SearchQuery { get; set; }
		public string SortColumn { get; set; }
		public string SortDirection { get; set; }
		public Pagination<SearchViewModel> Results { get; set; }

		public int FilterType { get; set; } // 0 - All, 1 - Task, 2 - Meeting, 3 - Reminder

		// Task filters
		public int? TaskStatusId { get; set; }
		public int? TaskPriorityId { get; set; }
		public int? TaskDifficultyId { get; set; }

		// Meeting filters
		public int? MeetingPriorityId { get; set; }
		public string MeetingLocation { get; set; }

		// Reminder filters
		public TimeOnly? ReminderTime { get; set; }

		public List<Status> Statuses { get; set; }
		public List<Priority> Priorities { get; set; }
		public List<Difficulty> Difficulties { get; set; }
	}

}
