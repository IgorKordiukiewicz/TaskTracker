namespace Shared;

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
