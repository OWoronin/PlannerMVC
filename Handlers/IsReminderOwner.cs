using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Pz_Proj_11_12.Data;
using Pz_Proj_11_12.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Pz_Proj_11_12.Handlers
{
    public class IsReminderOwnerRequirement : IAuthorizationRequirement { }

    public class IsReminderOwnerHandler : AuthorizationHandler<IsReminderOwnerRequirement, Reminder>
    {
        private readonly PlannerContext _context;
        public IsReminderOwnerHandler(PlannerContext context) { _context = context; }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            IsReminderOwnerRequirement requirement,
            Reminder reminder)
        {
            var userId = int.Parse(context.User.FindFirstValue(ClaimTypes.NameIdentifier));

            var day = await _context.Days.Where(d => d.Id == reminder.DayId).Include(d => d.Planner).FirstOrDefaultAsync();

            if (day?.Planner.UserId == userId)
            {
                context.Succeed(requirement);
            }
        }
    }
}
