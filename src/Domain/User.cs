using Domain.Common;

namespace Domain;

public class User : Entity<Guid>
{
    public string AuthenticationId { get; private set; } = string.Empty;

    private User(Guid id)
        : base(id)
    { }

    public static User Create(string authenticationId)
    {
        return new(Guid.NewGuid())
        {
            AuthenticationId = authenticationId
        };
    }
}
