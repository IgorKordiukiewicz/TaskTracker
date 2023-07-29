using Domain.Common;

namespace Domain.Organizations;


public class OrganizationMember : Entity<Guid>
{
    public Guid UserId { get; private init; }

    private OrganizationMember(Guid id)
        : base(id)
    {

    }

    public static OrganizationMember Create(Guid userId)
    {
        return new(Guid.NewGuid())
        {
            UserId = userId,
        };
    }
}