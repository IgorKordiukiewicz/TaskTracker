﻿namespace Domain.Users;

public class User : Entity, IAggregateRoot
{
    public string AuthenticationId { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;

    public string FullName => $"{FirstName} {LastName}";

    private User(Guid id)
        : base(id)
    { }

    public static User Create(string authenticationId, string email, string firstName, string lastName)
    {
        return new(Guid.NewGuid())
        {
            AuthenticationId = authenticationId,
            Email = email,
            FirstName = firstName,
            LastName = lastName
        };
    }

    public void UpdateName(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }
}
