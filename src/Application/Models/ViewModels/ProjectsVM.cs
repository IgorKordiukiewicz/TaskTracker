namespace Application.Models.ViewModels;

public record ProjectsVM(IReadOnlyList<ProjectVM> Projects);

public record ProjectVM
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required int MembersCount { get; init; }
    public required int TasksCount { get; init; }
}

public record ProjectInvitationsVM(IReadOnlyList<ProjectInvitationVM> Invitations);
public record ProjectInvitationVM(Guid Id, string UserEmail, ProjectInvitationState State, DateTime CreatedAt, DateTime? FinalizedAt, DateTime? ExpirationDate);

public record UserProjectInvitationsVM(IReadOnlyList<UserProjectInvitationVM> Invitations);
public record UserProjectInvitationVM(Guid Id, Guid ProjectId, string ProjectName);

public record ProjectSettingsVM(string Name, Guid OwnerId);

public record UserProjectPermissionsVM(ProjectPermissions Permissions, bool IsOwner);

public record ProjectMembersVM(IReadOnlyList<ProjectMemberVM> Members);
public record ProjectMemberVM
{
    public required Guid Id { get; init; }
    public required Guid UserId { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
    public required Guid RoleId { get; init; }
    public required string RoleName { get; init; }
    public required bool Owner { get; init; }
}

public record RolesVM(IReadOnlyList<RoleVM> Roles);

public record RoleVM
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required ProjectPermissions Permissions { get; init; }
    public required bool Modifiable { get; init; }
    public required bool Owner { get; init; }
}

public record ProjectInfoVM(Guid Id, string Name);