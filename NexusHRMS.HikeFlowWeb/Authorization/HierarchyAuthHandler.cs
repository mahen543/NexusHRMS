using HRMS_Common;
using HRMS_HikeFlow.NexusBAL;
using HRMS_HikeFlow.NexusProperties;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NexusHRMS.HikeFlowWeb.Authorization
{
    public class HierarchyRequirement : IAuthorizationRequirement
    {
        public byte MaxAllowedHierarchyLevel { get; }
        public long[] AllowedDesignationIds { get; }

        public HierarchyRequirement(byte maxAllowedHierarchyLevel, params long[] allowedDesignationIds)
        {
            MaxAllowedHierarchyLevel = maxAllowedHierarchyLevel;
            AllowedDesignationIds = allowedDesignationIds;
        }
    }

    public class HierarchyAuthorizationHandler : AuthorizationHandler<HierarchyRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HierarchyRequirement requirement)
        {
            var userPrincipal = context.User;

            if (userPrincipal?.Identity == null || !userPrincipal.Identity.IsAuthenticated)
            {
                return Task.CompletedTask;
            }

            // Extract claims populated by TokenGeneratorService (Zero DB Hits)
            var hierarchyClaim = userPrincipal.FindFirstValue("HierarchyLevel");
            var designationIdClaim = userPrincipal.FindFirstValue("DesignationId");
            var userIdClaim = userPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            var username = userPrincipal.FindFirstValue(ClaimTypes.Name) ?? string.Empty;

            if (!byte.TryParse(hierarchyClaim, out byte hierarchyLevel) ||
                !long.TryParse(designationIdClaim, out long designationId))
            {
                return Task.CompletedTask;
            }

            long.TryParse(userIdClaim, out long userId);

            var userContext = new UserHierarchyModel(userId, username, hierarchyLevel, designationId);

            // Execute in-memory RBAC evaluation via AuthBAL
            var authBal = new AuthBAL(null!);
            bool isAuthorized = authBal.ValidateHierarchyAuthorization(userContext, requirement.MaxAllowedHierarchyLevel, requirement.AllowedDesignationIds);

            if (isAuthorized)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}