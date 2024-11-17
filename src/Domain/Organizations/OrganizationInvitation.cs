namespace Domain.Organizations;

public class OrganizationInvitation : Entity
{
    public Guid UserId { get; private init; }
    public Guid OrganizationId { get; private init; }
    public OrganizationInvitationState State { get; private set; } = OrganizationInvitationState.Pending;
    public DateTime CreatedAt { get; private set; }
    public DateTime? FinalizedAt { get; private set; }

    private OrganizationInvitation(Guid id)
        : base(id)
    {

    }

    public static OrganizationInvitation Create(Guid userId, Guid organizationId)
    {
        return new(Guid.NewGuid())
        {
            UserId = userId,
            OrganizationId = organizationId,
            CreatedAt = DateTime.Now
        };
    }

    public void Accept()
    {
        State = OrganizationInvitationState.Accepted;
        FinalizedAt = DateTime.Now;
    }

    public void Decline()
    {
        State = OrganizationInvitationState.Declined;
        FinalizedAt = DateTime.Now;
    }

    public void Cancel()
    {
        State = OrganizationInvitationState.Canceled;
        FinalizedAt = DateTime.Now;
    }
}