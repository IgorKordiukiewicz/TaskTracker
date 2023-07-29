namespace Shared.ViewModels;

public record OrganizationsForUserVM(IReadOnlyList<OrganizationForUserVM> Organizations);

public record OrganizationForUserVM
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
}
