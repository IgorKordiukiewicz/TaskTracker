namespace Shared;

public static class Policy
{
    // TODO: find better name than editor, Manager maybe?
    // TODO: use nameof ?
    public const string OrganizationMember = "OrganizationMember";
    public const string OrganizationMembersEditor = "OrganizationMembersEditor";
    public const string OrganizationProjectsEditor = "OrganizationProjectsEditor";

    public const string ProjectMember = "ProjectMember";
    public const string ProjectMembersEditor = "ProjectMembersEditor";
    public const string ProjectTasksEditor = "ProjectTasksEditor";
    public const string ProjectWorkflowsEditor = "ProjectWorkflowsEditor";
}
