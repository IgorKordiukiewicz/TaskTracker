﻿namespace Shared.Dtos;

public record UserRegistrationDto(string Email, string FirstName, string LastName);

public record UpdateUserNameDto(string FirstName, string LastName);
