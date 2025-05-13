using Pz_Proj_11_12.Models.LookupTables;

namespace Pz_Proj_11_12.Models
{
    public class Meeting
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Priority Priority { get; set; }
        public int PriorityId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Location { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }

        //in final project will add option to add reminder for meeting in meeting view 
        public int DayId { get; set; }
        public Day Day { get; set; } = null!;

    }
}
