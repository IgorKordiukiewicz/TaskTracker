using Microsoft.AspNetCore.Authorization;
using Shared.Enums;

namespace Shared.Authorization;

public static class Policy
{
    public const string OrganizationMember = nameof(OrganizationMember);
    public const string OrganizationInviteMembers = nameof(OrganizationInviteMembers);
    public const string OrganizationRemoveMembers = nameof(OrganizationRemoveMembers);
    public const string OrganizationCreateProjects = nameof(OrganizationCreateProjects);
    public const string OrganizationManageRoles = nameof(OrganizationManageRoles);

    public const string ProjectMember = nameof(ProjectMember);
    public const string ProjectAddMembers = nameof(ProjectAddMembers);
    public const string ProjectRemoveMembers = nameof(ProjectRemoveMembers);
    public const string ProjectCreateTasks = nameof(ProjectCreateTasks);
    public const string ProjectModifyTasks = nameof(ProjectModifyTasks);
    public const string ProjectTransitionTasks = nameof(ProjectTransitionTasks);
    public const string ProjectAssignTasks = nameof(ProjectAssignTasks);
    public const string ProjectAddComments = nameof(ProjectAddComments);
    public const string ProjectManageWorkflows = nameof(ProjectManageWorkflows);
    public const string ProjectManageProject = nameof(ProjectManageProject);
    public const string ProjectManageRoles = nameof(ProjectManageRoles);
    public const string ProjectLogTimeOnTasks = nameof(ProjectLogTimeOnTasks);
    public const string ProjectEstimateTasks = nameof(ProjectEstimateTasks);

    public const string UserSelf = nameof(UserSelf);
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
        options.AddPolicy(Policy.ProjectAddMembers, policy => policy.Requirements.Add(new ProjectMemberRequirement(ProjectPermissions.AddMembers)));
        options.AddPolicy(Policy.ProjectRemoveMembers, policy => policy.Requirements.Add(new ProjectMemberRequirement(ProjectPermissions.RemoveMembers)));
        options.AddPolicy(Policy.ProjectCreateTasks, policy => policy.Requirements.Add(new ProjectMemberRequirement(ProjectPermissions.CreateTasks)));
        options.AddPolicy(Policy.ProjectModifyTasks, policy => policy.Requirements.Add(new ProjectMemberRequirement(ProjectPermissions.ModifyTasks)));
        options.AddPolicy(Policy.ProjectTransitionTasks, policy => policy.Requirements.Add(new ProjectMemberRequirement(ProjectPermissions.TransitionTasks)));
        options.AddPolicy(Policy.ProjectAssignTasks, policy => policy.Requirements.Add(new ProjectMemberRequirement(ProjectPermissions.AssignTasks)));
        options.AddPolicy(Policy.ProjectAddComments, policy => policy.Requirements.Add(new ProjectMemberRequirement(ProjectPermissions.AddComments)));
        options.AddPolicy(Policy.ProjectManageWorkflows, policy => policy.Requirements.Add(new ProjectMemberRequirement(ProjectPermissions.ManageWorkflows)));
        options.AddPolicy(Policy.ProjectManageProject, policy => policy.Requirements.Add(new ProjectMemberRequirement(ProjectPermissions.ManageProject)));
        options.AddPolicy(Policy.ProjectManageRoles, policy => policy.Requirements.Add(new ProjectMemberRequirement(ProjectPermissions.ManageRoles)));
        options.AddPolicy(Policy.ProjectLogTimeOnTasks, policy => policy.Requirements.Add(new ProjectMemberRequirement(ProjectPermissions.LogTimeOnTasks)));
        options.AddPolicy(Policy.ProjectEstimateTasks, policy => policy.Requirements.Add(new ProjectMemberRequirement(ProjectPermissions.EstimateTasks)));

        options.AddPolicy(Policy.UserSelf, policy => policy.Requirements.Add(new UserSelfRequirement()));
    }
}
