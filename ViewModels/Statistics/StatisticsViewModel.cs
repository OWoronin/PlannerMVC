namespace Pz_Proj_11_12.ViewModels.Statistics
{
    public class StatisticsViewModel
    {
        public int amountOfTasks { get; set; }
        public int amountOfMeetings { get; set; }
        public int amountOfReminders { get; set; }
        public int amountOfRemindersBefore12 { get; set; }
        public int amountOfRemindersAfter12 { get; set; }
        public int amountOfMeetingsBefore12 { get; set; }
        public int amountOfMeetingsAfter12 { get; set; }

        public List<DayViewModel> Days { get; set; }
        public List<DifficultyViewModel> Difficulties { get; set; }
        public List<LocationViewModel> Locations { get; set; }
        public List<PriorityViewModel> Priorities { get; set; }
        public List<StatusViewModel> Statuses { get; set; }




    }
}
