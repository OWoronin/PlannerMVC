using Pz_Proj_11_12.Models.LookupTables;
using System.ComponentModel.DataAnnotations;

namespace Pz_Proj_11_12.Models
{
    public class Reminder
    {
        public int Id { get; set; }
        [Required]
        [StringLength(11)]
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public TimeOnly ReminderTime { get; set; }
        public int DayId { get; set; }
        public Day Day { get; set; } = null!;

    }
}
