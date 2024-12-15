namespace Domain.Organizations;

public class OrganizationInvitation : Entity
{
    public Guid UserId { get; private init; }
    public Guid OrganizationId { get; private init; }
    public OrganizationInvitationState State { get; private set; } = OrganizationInvitationState.Pending;
    public DateTime CreatedAt { get; private set; }
    public DateTime? FinalizedAt { get; private set; }
    public DateTime? ExpirationDate { get; private set; }

    private OrganizationInvitation(Guid id)
        : base(id)
    {

    }

    public static OrganizationInvitation Create(Guid userId, Guid organizationId, DateTime now, DateTime? expirationDate)
    {
        return new(Guid.NewGuid())
        {
            UserId = userId,
            OrganizationId = organizationId,
            CreatedAt = now,
            ExpirationDate = expirationDate
        };
    }

    public void Accept(DateTime now)
    {
        State = OrganizationInvitationState.Accepted;
        FinalizedAt = now;
    }

    public void Decline(DateTime now)
    {
        State = OrganizationInvitationState.Declined;
        FinalizedAt = now;
    }

    public void Cancel(DateTime now)
    {
        State = OrganizationInvitationState.Canceled;
        FinalizedAt = now;
    }

    public void Expire(DateTime now)
    {
        if(!ExpirationDate.HasValue || ExpirationDate.Value > now || State != OrganizationInvitationState.Pending)
        {
            return;
        }

        State = OrganizationInvitationState.Expired;
        FinalizedAt = now;
    }
}