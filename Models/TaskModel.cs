using Pz_Proj_11_12.Models.LookupTables;

namespace Pz_Proj_11_12.Models
{
    public class TaskModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Difficulty Difficulty { get; set; } //object
        public int DifficultyId { get; set; } //foreign key
        public Priority Priority { get; set; }
        public int PriorityId { get; set; } 
        public Status Status { get; set; }
        public int StatusId { get; set; }
        public DateTime CreatedDate { get; set; }

        //final project will have boolean fields rescheduled and partial, as well deadline will be added in feature
        public int  DayId { get; set; }
        public Day Day { get; set; } = null!;



    }
}
