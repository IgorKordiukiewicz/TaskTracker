namespace Shared.ViewModels;

public record UserVM(Guid Id, string Name, string Email, IReadOnlyList<Guid> OrganizationsMember, IReadOnlyList<Guid> ProjectsMember); // TODO: Dictionary with list of permissions?
