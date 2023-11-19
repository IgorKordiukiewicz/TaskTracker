namespace Shared.ViewModels;

public interface IMemberVM
{
    Guid Id { get; }
    string Name { get; }
}

public record OrganizationMembersVM(IReadOnlyList<OrganizationMemberVM> Members);
public record OrganizationMemberVM(Guid Id, string Name, bool Owner) : IMemberVM;

public record ProjectMembersVM(IReadOnlyList<ProjectMemberVM> Members);
public record ProjectMemberVM(Guid Id, Guid UserId, string Name) : IMemberVM;