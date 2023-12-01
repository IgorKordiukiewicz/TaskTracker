using Microsoft.AspNetCore.Authorization;
using Shared.Enums;

namespace Shared.Authorization;

public static class Policy
{
    // TODO: find better name than editor, Manager maybe?
    public const string OrganizationMember = nameof(OrganizationMember);
    public const string OrganizationInviteMembers = nameof(OrganizationInviteMembers);
    public const string OrganizationRemoveMembers = nameof(OrganizationRemoveMembers);
    public const string OrganizationCreateProjects = nameof(OrganizationCreateProjects);
    public const string OrganizationManageRoles = nameof(OrganizationManageRoles);

    public const string ProjectMember = nameof(ProjectMember);
    public const string ProjectMembersEditor = nameof(ProjectMembersEditor);
    public const string ProjectTasksEditor = nameof(ProjectTasksEditor);
    public const string ProjectWorkflowsEditor = nameof(ProjectWorkflowsEditor);
    public const string ProjectSettingsEditor = nameof(ProjectSettingsEditor);
}

public static class PolicyExtensions
{
    public static void AddPolicies(this AuthorizationOptions options)
    {
        options.AddPolicy(Policy.OrganizationMember, policy => policy.Requirements.Add(new OrganizationMemberRequirement()));
        options.AddPolicy(Policy.OrganizationInviteMembers, policy => policy.Requirements.Add(new OrganizationMemberRequirement(OrganizationPermissions.InviteMembers)));
        options.AddPolicy(Policy.OrganizationRemoveMembers, policy => policy.Requirements.Add(new OrganizationMemberRequirement(OrganizationPermissions.RemoveMembers)));
        options.AddPolicy(Policy.OrganizationCreateProjects, policy => policy.Requirements.Add(new OrganizationMemberRequirement(OrganizationPermissions.CreateProjects)));
        options.AddPolicy(Policy.OrganizationManageRoles, policy => policy.Requirements.Add(new OrganizationMemberRequirement(OrganizationPermissions.ManageRoles)));

        options.AddPolicy(Policy.ProjectMember, policy => policy.Requirements.Add(new ProjectMemberRequirement()));
        options.AddPolicy(Policy.ProjectMembersEditor, policy => policy.Requirements.Add(new ProjectMemberRequirement(ProjectPermissions.Members)));
        options.AddPolicy(Policy.ProjectTasksEditor, policy => policy.Requirements.Add(new ProjectMemberRequirement(ProjectPermissions.Tasks)));
        options.AddPolicy(Policy.ProjectWorkflowsEditor, policy => policy.Requirements.Add(new ProjectMemberRequirement(ProjectPermissions.Workflows)));
    }
}
