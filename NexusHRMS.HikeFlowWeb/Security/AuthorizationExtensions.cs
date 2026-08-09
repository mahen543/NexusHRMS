using NexusHRMS.HikeFlowWeb.Authorization;

namespace NexusHRMS.HikeFlowWeb.Security
{
    public static class AuthorizationExtensions
    {
        public static IServiceCollection AddAppAuthorization(this IServiceCollection services)
        {
            var builder = services.AddAuthorizationBuilder();

            /* ==========================================
               LEVEL 1: Executive & Administrative
            ========================================== */

            // Helper to register policy safely using generic Enum.GetName<TEnum>()
            builder.AddPolicy(Enum.GetName(Designations.Admin)!, policy =>
                policy.Requirements.Add(new HierarchyRequirement(maxAllowedHierarchyLevel: 1)));

            /* ==========================================
               LEVEL 2: Department Heads & Managers
            ========================================== */

            builder.AddPolicy("Manager", policy =>
                policy.Requirements.Add(new HierarchyRequirement(maxAllowedHierarchyLevel: 2)));

            builder.AddPolicy(Enum.GetName(Designations.HRManager)!, policy =>
                policy.Requirements.Add(new HierarchyRequirement(maxAllowedHierarchyLevel: 2, Designations.HRManager)));

            builder.AddPolicy(Enum.GetName(Designations.ProjectManager)!, policy =>
                policy.Requirements.Add(new HierarchyRequirement(maxAllowedHierarchyLevel: 2, Designations.ProjectManager)));

            /* ==========================================
               LEVEL 3: Mid-Level / Team Leads / Specialists
            ========================================== */

            builder.AddPolicy(Enum.GetName(Designations.TeamLead)!, policy =>
                policy.Requirements.Add(new HierarchyRequirement(maxAllowedHierarchyLevel: 3, Designations.TeamLead)));

            builder.AddPolicy(Enum.GetName(Designations.AssistantHR)!, policy =>
                policy.Requirements.Add(new HierarchyRequirement(maxAllowedHierarchyLevel: 3, Designations.AssistantHR)));

            builder.AddPolicy(Enum.GetName(Designations.Accountant)!, policy =>
                policy.Requirements.Add(new HierarchyRequirement(maxAllowedHierarchyLevel: 3, Designations.Accountant)));

            builder.AddPolicy(Enum.GetName(Designations.QualityEngineer)!, policy =>
                policy.Requirements.Add(new HierarchyRequirement(maxAllowedHierarchyLevel: 3, Designations.QualityEngineer)));

            /* ==========================================
               LEVEL 4: General Staff
            ========================================== */

            builder.AddPolicy(Enum.GetName(Designations.Developer)!, policy =>
                policy.Requirements.Add(new HierarchyRequirement(maxAllowedHierarchyLevel: 4, Designations.Developer)));

            builder.AddPolicy("GeneralEmployee", policy =>
                policy.Requirements.Add(new HierarchyRequirement(maxAllowedHierarchyLevel: 4)));

            return services;
        }
    }
}
