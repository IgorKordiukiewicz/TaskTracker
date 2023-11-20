using Domain.Common;

namespace Domain.Organizations;


public class OrganizationMember : Entity, IHasRole
{
    public Guid UserId { get; private init; }
    public Guid RoleId { get; private set; }

    private OrganizationMember(Guid id)
        : base(id)
    {

    }

    public static OrganizationMember Create(Guid userId, Guid roleId)
    {
        return new(Guid.NewGuid())
        {
            UserId = userId,
            RoleId = roleId
        };
    }

    public void UpdateRole(Guid roleId)
    {
        RoleId = roleId;
    }
}