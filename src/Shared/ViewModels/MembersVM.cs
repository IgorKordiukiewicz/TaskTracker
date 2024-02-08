namespace Shared.ViewModels;

public interface IMemberVM
{
    Guid Id { get; }
    Guid UserId { get; }
    string Name { get; }
    Guid RoleId { get; }
}

public record OrganizationMembersVM(IReadOnlyList<OrganizationMemberVM> Members);
public record OrganizationMemberVM(Guid Id, Guid UserId, string Name, Guid RoleId, bool Owner) : IMemberVM;

public record ProjectMembersVM(IReadOnlyList<ProjectMemberVM> Members);
public record ProjectMemberVM(Guid Id, Guid UserId, string Name, Guid RoleId) : IMemberVM;