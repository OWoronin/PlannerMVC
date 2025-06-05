using Newtonsoft.Json.Serialization;
using Pz_Proj_11_12.Models.LookupTables;
using System.ComponentModel.DataAnnotations;

namespace Pz_Proj_11_12.Models
{
    public class Meeting
    {
        public int Id { get; set; }

        [Required]
        [StringLength(9)]
        public string Name { get; set; }
        public string Description { get; set; }
        public Priority Priority { get; set; }
        public int PriorityId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Location { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int DayId { get; set; }
        public Day Day { get; set; } = null!;

    }
}
