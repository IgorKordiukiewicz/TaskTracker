using Shared.Enums;

namespace Shared.ViewModels;

public record UserVM
{
    public required Guid Id { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string FullName { get; init; }
    public required string Email { get; init; }
}

public record UsersSearchVM(IReadOnlyList<UserSearchVM> Users);

public record UserSearchVM
{
    public required Guid Id { get; init; }
    public required string Email { get; init; }
}

public record UsersPresentationDataVM(IReadOnlyList<UserPresentationDataVM> Users);
public record UserPresentationDataVM(Guid UserId, string FirstName, string LastName, string AvatarColor);