namespace Shared.Dtos;

public record UserRegistrationDto(Guid Id, string Email, string FirstName, string LastName, string AvatarColor);

public record UpdateUserNameDto(string FirstName, string LastName);
