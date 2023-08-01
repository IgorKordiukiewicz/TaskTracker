using Domain.Common;

namespace Domain.Organizations;

public enum OrganizationInvitationState
{
    Pending,
    Accepted,
    Declined,
    // TODO: Expired, Canceled?
}

public class OrganizationInvitation : Entity<Guid>
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

    public void Decline()
    {
        State = OrganizationInvitationState.Declined;
    }
}