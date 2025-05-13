using Microsoft.EntityFrameworkCore;
using Pz_Proj_11_12.Models;
using Pz_Proj_11_12.Models.LookupTables;

namespace Pz_Proj_11_12.Data
{
    public class PlannerContext(DbContextOptions<PlannerContext> options) : DbContext(options)
    {
        public DbSet<Planner> Planners { get; set; }
        public DbSet<Day> Days { get; set; }
        public DbSet<TaskModel> Tasks { get; set; }
        public DbSet<Reminder> Reminders { get; set; }
        public DbSet<Meeting> Meetings { get; set; }

        //lookups are db sets too 
        public DbSet<Priority> Priorities { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<Difficulty> Difficulties { get; set; }

        
    }
    
}
