using Microsoft.AspNetCore.Authorization;
using Pz_Proj_11_12.Models;
using System.Security.Claims;

namespace Pz_Proj_11_12.Handlers
{
    public class IsPlannerOwnerRequirement : IAuthorizationRequirement { }

    public class IsPlannerOwnerHandler : AuthorizationHandler<IsPlannerOwnerRequirement, Planner>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            IsPlannerOwnerRequirement requirement,
            Planner planner)
        {
            var userId = int.Parse(context.User.FindFirstValue(ClaimTypes.NameIdentifier));
            //if (planner.UserId == userId + 1000) TODO: for test
            if (planner.UserId == userId)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
