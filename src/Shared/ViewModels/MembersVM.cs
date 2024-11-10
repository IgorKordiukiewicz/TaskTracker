namespace Shared.ViewModels;

public interface IMemberVM
{
    Guid Id { get; }
    Guid UserId { get; }
    string Name { get; }
    Guid RoleId { get; }
}

public record OrganizationMembersVM(IReadOnlyList<OrganizationMemberVM> Members);
public record OrganizationMemberVM : IMemberVM
{
    public required Guid Id { get; init; }
    public required Guid UserId { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
    public required Guid RoleId { get; init; }
    public required string RoleName { get; init; }
    public required bool Owner { get; init; }
}

public record ProjectMembersVM(IReadOnlyList<ProjectMemberVM> Members);
public record ProjectMemberVM(Guid Id, Guid UserId, string Name, Guid RoleId) : IMemberVM;