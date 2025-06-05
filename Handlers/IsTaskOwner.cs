using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Pz_Proj_11_12.Data;
using Pz_Proj_11_12.Models;
using System.Security.Claims;

namespace Pz_Proj_11_12.Handlers
{
    public class IsTaskOwnerRequirement : IAuthorizationRequirement { }

    public class IsTaskOwnerHandler : AuthorizationHandler<IsTaskOwnerRequirement, TaskModel>
    {
        private readonly PlannerContext _context;
        public IsTaskOwnerHandler(PlannerContext context) { _context = context; }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            IsTaskOwnerRequirement requirement,
            TaskModel task)
        {
            var userId = int.Parse(context.User.FindFirstValue(ClaimTypes.NameIdentifier));

            var day = await _context.Days.Where(d=>d.Id == task.DayId).Include(d => d.Planner).FirstOrDefaultAsync();

            if (day?.Planner.UserId == userId)
            {
                context.Succeed(requirement);
            }
        }
    }

}
