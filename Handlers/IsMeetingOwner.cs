using Microsoft.AspNetCore.Authorization;
using Pz_Proj_11_12.Data;
using Microsoft.EntityFrameworkCore;
using Pz_Proj_11_12.Models;
using System.Security.Claims;

namespace Pz_Proj_11_12.Handlers
{
    public class IsMeetingOwnerRequirement : IAuthorizationRequirement { }

    public class IsMeetingOwnerHandler : AuthorizationHandler<IsMeetingOwnerRequirement, Meeting>
    {
        private readonly PlannerContext _context;
        public IsMeetingOwnerHandler(PlannerContext context) { _context = context; }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            IsMeetingOwnerRequirement requirement,
            Meeting meeting)
        {
            var userId = int.Parse(context.User.FindFirstValue(ClaimTypes.NameIdentifier));

            var day = await _context.Days.Where(d => d.Id == meeting.DayId).Include(d => d.Planner).FirstOrDefaultAsync();

            if (day?.Planner.UserId == userId)
            {
                context.Succeed(requirement);
            }
        }
    }
}
