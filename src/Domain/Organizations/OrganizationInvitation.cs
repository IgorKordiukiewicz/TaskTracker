using Domain.Common;
using Shared.Enums;

namespace Domain.Organizations;

public class OrganizationInvitation : Entity
{
    public Guid UserId { get; private init; }
    public Guid OrganizationId { get; private init; }
    public OrganizationInvitationState State { get; private set; } = OrganizationInvitationState.Pending;

    private OrganizationInvitation(Guid id)
        : base(id)
    {

    }

    public static OrganizationInvitation Create(Guid userId, Guid organizationId)
    {
        return new(Guid.NewGuid())
        {
            UserId = userId,
            OrganizationId = organizationId
        };
    }

    public void Accept()
    {
        State = OrganizationInvitationState.Accepted;
    }

    public void Decline()
    {
        State = OrganizationInvitationState.Declined;
    }
}