using Microsoft.AspNetCore.Authorization;
using Shared.Enums;

namespace Shared.Authorization;

public static class Policy
{
    public const string OrganizationMember = nameof(OrganizationMember);
    public const string OrganizationEditMembers = nameof(OrganizationEditMembers);
    public const string OrganizationEditProjects = nameof(OrganizationEditProjects);
    public const string OrganizationEditRoles = nameof(OrganizationEditRoles);
    public const string OrganizationEditOrganization = nameof(OrganizationEditOrganization);
    public const string OrganizationOwner = nameof(OrganizationOwner);

    public const string ProjectMember = nameof(ProjectMember);
    public const string ProjectEditTasks = nameof(ProjectEditTasks);
    public const string ProjectEditMembers = nameof(ProjectEditMembers);
    public const string ProjectEditRoles = nameof(ProjectEditRoles);
    public const string ProjectEditProject = nameof(ProjectEditProject);

    public const string UserSelf = nameof(UserSelf);
}

public static class PolicyExtensions
{
    public static void AddPolicies(this AuthorizationOptions options)
    {
        options.AddPolicy(Policy.OrganizationMember, policy => policy.Requirements.Add(new OrganizationMemberRequirement()));
        options.AddPolicy(Policy.OrganizationEditMembers, policy => policy.Requirements.Add(new OrganizationMemberRequirement(OrganizationPermissions.EditMembers)));
        options.AddPolicy(Policy.OrganizationEditProjects, policy => policy.Requirements.Add(new OrganizationMemberRequirement(OrganizationPermissions.EditProjects)));
        options.AddPolicy(Policy.OrganizationEditRoles, policy => policy.Requirements.Add(new OrganizationMemberRequirement(OrganizationPermissions.EditRoles)));
        options.AddPolicy(Policy.OrganizationEditOrganization, policy => policy.Requirements.Add(new OrganizationMemberRequirement(OrganizationPermissions.EditOrganization)));
        options.AddPolicy(Policy.OrganizationOwner, policy => policy.Requirements.Add(new OrganizationMemberRequirement(owner: true)));

        options.AddPolicy(Policy.ProjectMember, policy => policy.Requirements.Add(new ProjectMemberRequirement()));
        options.AddPolicy(Policy.ProjectEditTasks, policy => policy.Requirements.Add(new ProjectMemberRequirement(ProjectPermissions.EditTasks)));
        options.AddPolicy(Policy.ProjectEditMembers, policy => policy.Requirements.Add(new ProjectMemberRequirement(ProjectPermissions.EditMembers)));
        options.AddPolicy(Policy.ProjectEditRoles, policy => policy.Requirements.Add(new ProjectMemberRequirement(ProjectPermissions.EditRoles)));
        options.AddPolicy(Policy.ProjectEditProject, policy => policy.Requirements.Add(new ProjectMemberRequirement(ProjectPermissions.EditProject)));

        options.AddPolicy(Policy.UserSelf, policy => policy.Requirements.Add(new UserSelfRequirement()));
    }
}
