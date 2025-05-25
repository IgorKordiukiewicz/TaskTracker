namespace Domain.Projects;

public class ProjectInvitation : Entity
{
    public Guid UserId { get; private init; }
    public Guid ProjectId { get; private init; }
    public ProjectInvitationState State { get; private set; } = ProjectInvitationState.Pending;
    public DateTime CreatedAt { get; private set; }
    public DateTime? FinalizedAt { get; private set; }
    public DateTime? ExpirationDate { get; private set; }

    private ProjectInvitation(Guid id)
        : base(id)
    {

    }

    public static ProjectInvitation Create(Guid userId, Guid projectId, DateTime now, DateTime? expirationDate)
    {
        return new(Guid.NewGuid())
        {
            UserId = userId,
            ProjectId = projectId,
            CreatedAt = now,
            ExpirationDate = expirationDate
        };
    }

    public void Accept(DateTime now)
    {
        State = ProjectInvitationState.Accepted;
        FinalizedAt = now;
    }

    public void Decline(DateTime now)
    {
        State = ProjectInvitationState.Declined;
        FinalizedAt = now;
    }

    public void Cancel(DateTime now)
    {
        State = ProjectInvitationState.Canceled;
        FinalizedAt = now;
    }

    public bool Expire(DateTime now)
    {
        if (!ExpirationDate.HasValue || ExpirationDate.Value > now || State != ProjectInvitationState.Pending)
        {
            return false;
        }

        State = ProjectInvitationState.Expired;
        FinalizedAt = now;
        return true;
    }
}
