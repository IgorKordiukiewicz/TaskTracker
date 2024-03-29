﻿namespace Infrastructure.Models;

public class UserPresentationData
{
    public required Guid UserId { get; init; }
    public required string AvatarColor { get; init; }
}
