namespace Domain.Users;

public class User : Entity, IAggregateRoot
{
    public string Email { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;

    public string FullName => $"{FirstName} {LastName}";

    private User(Guid id)
        : base(id)
    { }

    public static User Create(Guid id, string email, string firstName, string lastName)
    {
        return new(id)
        {
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
