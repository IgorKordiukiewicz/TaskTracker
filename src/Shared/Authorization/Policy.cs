using Microsoft.AspNetCore.Authorization;
using Shared.Enums;

namespace Shared.Authorization;

public static class Policy
{
    // TODO: find better name than editor, Manager maybe?
    public const string OrganizationMember = nameof(OrganizationMember);
    public const string OrganizationMembersEditor = nameof(OrganizationMembersEditor);
    public const string OrganizationProjectsEditor = nameof(OrganizationProjectsEditor);

    public const string ProjectMember = nameof(ProjectMember);
    public const string ProjectMembersEditor = nameof(ProjectMembersEditor);
    public const string ProjectTasksEditor = nameof(ProjectTasksEditor);
    public const string ProjectWorkflowsEditor = nameof(ProjectWorkflowsEditor);
}

public static class PolicyExtensions
{
    public static void AddPolicies(this AuthorizationOptions options)
    {
        options.AddPolicy(Policy.OrganizationMember, policy => policy.Requirements.Add(new OrganizationMemberRequirement()));
        options.AddPolicy(Policy.OrganizationMembersEditor, policy => policy.Requirements.Add(new OrganizationMemberRequirement(OrganizationPermissions.Members)));
        options.AddPolicy(Policy.OrganizationProjectsEditor, policy => policy.Requirements.Add(new OrganizationMemberRequirement(OrganizationPermissions.Projects)));

        options.AddPolicy(Policy.ProjectMember, policy => policy.Requirements.Add(new ProjectMemberRequirement()));
        options.AddPolicy(Policy.ProjectMembersEditor, policy => policy.Requirements.Add(new ProjectMemberRequirement(ProjectPermissions.Members)));
        options.AddPolicy(Policy.ProjectTasksEditor, policy => policy.Requirements.Add(new ProjectMemberRequirement(ProjectPermissions.Tasks)));
        options.AddPolicy(Policy.ProjectWorkflowsEditor, policy => policy.Requirements.Add(new ProjectMemberRequirement(ProjectPermissions.Workflows)));
    }
}
