using Domain.Common;

namespace Domain.Users;

public class User : Entity<Guid>
{
    public string AuthenticationId { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    private User(Guid id)
        : base(id)
    { }

    public static User Create(string authenticationId, string name)
    {
        return new(Guid.NewGuid())
        {
            AuthenticationId = authenticationId,
            Name = name
        };
    }
}
