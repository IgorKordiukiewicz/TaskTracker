namespace Shared.ViewModels;

public record OrganizationMembersVM(IReadOnlyList<OrganizationMemberVM> Members);
public record OrganizationMemberVM(Guid Id, string Name, bool Owner);
