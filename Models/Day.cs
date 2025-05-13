namespace Pz_Proj_11_12.Models
{
    public class Day
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<TaskModel> Tasks { get; set; } = new List<TaskModel>();
        public ICollection<Reminder>  Reminders { get; set; } = new List<Reminder>();
        public ICollection<Meeting> Meetings { get; set; } = new List<Meeting>();

        public int PlannerId { get; set; }
        public Planner Planner { get; set; } = null!; //required ref navigation 

    }
}
