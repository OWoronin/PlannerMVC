using System.ComponentModel.DataAnnotations;

namespace Pz_Proj_11_12.ViewModels.Meeting
{
    public class CreateMeetingViewModel
    {
		[Required]
		[StringLength(9)]
		public string Name { get; set; }
		public string Description { get; set; }

		public int PriorityId { get; set; }

		public string Location { get; set; }
		public TimeOnly StartTime { get; set; }
		public TimeOnly EndTime { get; set; }

		public int DayId { get; set; }

		public bool IsWithReminder { get; set; }
		[StringLength(9)]
		public string? ReminderName { get; set; }

		public int? HoursBefore { get; set; }
	}
}
