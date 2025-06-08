using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Pz_Proj_11_12.Data;
using Pz_Proj_11_12.Models;
using System.Security.Claims;

namespace Pz_Proj_11_12.Handlers
{
    public class IsDayOwnerRequirement : IAuthorizationRequirement { }

    public class IsDayOwnerHandler : AuthorizationHandler<IsDayOwnerRequirement, Day>
    {
        private readonly PlannerContext _context;

        public IsDayOwnerHandler(PlannerContext context)
        {
            _context = context;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            IsDayOwnerRequirement requirement,
            Day day)
        {
            var userId = int.Parse(context.User.FindFirstValue(ClaimTypes.NameIdentifier)); //id zalog


            Planner? planner = day.Planner;
            if (planner == null)
            {
                planner = await _context.Planners.FirstOrDefaultAsync(p => p.Id == day.PlannerId); //planner z dnia pobieram z db
            }

            //throw new Exception($"userId: {userId}, dayId: {day.Id}, plannerId: {planner.Id}, planner contains day: {planner.Days.Any(d => d.Id == day.Id)}");
            if (planner != null && planner.UserId == userId)
            {
                context.Succeed(requirement);
            }
        }
    }
}
