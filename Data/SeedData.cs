using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pz_Proj_11_12.Models;
using Pz_Proj_11_12.Models.LookupTables;

namespace Pz_Proj_11_12.Data
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var context = new PlannerContext(
                serviceProvider.GetRequiredService<DbContextOptions<PlannerContext>>());

            using var transaction = context.Database.BeginTransaction();

            try
            {
                // Seed lookup tables
                if (!context.Priorities.Any())
                {
                    context.Priorities.AddRange(
                        new Priority { Name = "Low" },
                        new Priority { Name = "Medium" },
                        new Priority { Name = "High" }
                    );
                    context.SaveChanges();
                }

                if (!context.Difficulties.Any())
                {
                    context.Difficulties.AddRange(
                        new Difficulty { Name = "Low" },
                        new Difficulty { Name = "Medium" },
                        new Difficulty { Name = "Hard" }
                    );
                    context.SaveChanges();
                }



                if (!context.Statuses.Any())
                {
                    context.Statuses.AddRange(
                        new Status { Name = "Created" },
                        new Status { Name = "In_Progress" },
                        new Status { Name = "Finished" }
                    );
                    context.SaveChanges();
                }

               
                var highPriorityId = context.Priorities.First(p => p.Name == "High").Id;
                var mediumPriorityId = context.Priorities.First(p => p.Name == "Medium").Id;

                var mediumDifficultyId = context.Difficulties.First(d => d.Name == "Medium").Id;
                var createdStatusId = context.Statuses.First(s => s.Name == "Created").Id;

                User testUser; 

                if (!context.Users.Any())
                {
                    testUser = new User { Login = "test", Password = "test" };
                    context.Users.Add(testUser);
                    context.SaveChanges();
                }
                else
                {
                    testUser = context.Users.First();
                }

                if (!context.Planners.Any())
                {
                    var planner = new Planner { Name = "Weekly Planner", UserId = testUser.Id };
                    context.Planners.Add(planner);
                    context.SaveChanges();

                    string[] dayNames = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"];

                    for (int i = 0; i < 7; i++)
                    {
                        var day = new Day
                        {
                            Name = dayNames[i],
                            PlannerId = planner.Id
                        };
                        context.Days.Add(day);
                        context.SaveChanges();

                        context.Tasks.Add(new TaskModel
                        {
                            Name = $"Task {i + 1}",
                            Description = "Sample task description.",
                            CreatedDate = DateTime.UtcNow,
                            PriorityId = mediumPriorityId,
                            DifficultyId = mediumDifficultyId,
                            StatusId = createdStatusId,
                            DayId = day.Id
                        });

                        context.Reminders.Add(new Reminder
                        {
                            Name = $"Reminder {i + 1}",
                            Description = "Sample reminder description.",
                            CreatedDate = DateTime.UtcNow,
                            ReminderTime = new TimeOnly(9, 0),
                            DayId = day.Id
                        });

                        context.Meetings.Add(new Meeting
                        {
                            Name = $"Meeting {i + 1}",
                            Description = "Sample meeting description.",
                            CreatedDate = DateTime.UtcNow,
                            PriorityId = highPriorityId,
                            Location = "Conference Room",
                            StartTime = new TimeOnly(10, 0),
                            EndTime = new TimeOnly(11, 0),
                            DayId = day.Id
                        });

                        context.SaveChanges();
                    }
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine("SEED ERROR: " + ex.Message);
                throw;
            }
        }
    }
}
