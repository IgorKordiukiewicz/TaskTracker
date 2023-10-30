namespace Shared.ViewModels;

public record UsersSearchVM(IReadOnlyList<UserSearchVM> Users);

public record UserSearchVM
{
    public required Guid Id { get; init; }
    public required string Email { get; init; }
}
