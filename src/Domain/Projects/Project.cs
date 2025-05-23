using Domain.Enums;

namespace Domain.Projects;

public class Project : Entity, IAggregateRoot, IHasName
{
    public string Name { get; set; } = string.Empty;
    public Guid OwnerId { get; private init; }

    private readonly List<ProjectMember> _members = [];
    public IReadOnlyList<ProjectMember> Members => _members.AsReadOnly();

    private readonly List<ProjectInvitation> _invitations = [];
    public IReadOnlyList<ProjectInvitation> Invitations => _invitations.AsReadOnly();

    private readonly List<ProjectRole> _roles = [];
    public IReadOnlyList<ProjectRole> Roles => _roles.AsReadOnly();

    public RolesManager<ProjectRole, ProjectPermissions> RolesManager { get; private init; }

    private Project(Guid id)
        : base(id)
    {
        RolesManager = new(_roles, (name, permissions) => new ProjectRole(name, Id, permissions));
    }

    public static Project Create(string name, Guid ownerId)
    {
        var result = new Project(Guid.NewGuid())
        {
            Name = name,
            OwnerId = ownerId,
        };

        result._roles.AddRange(ProjectRole.CreateDefaultRoles(result.Id));

        var member = ProjectMember.Create(ownerId, result.RolesManager.GetOwnerRoleId()!.Value);
        result._members.Add(member);

        return result;
    }

    public Result<ProjectInvitation> CreateInvitation(Guid userId, DateTime now, int? expirationDays = null)
    {
        if (_members.Any(x => x.UserId == userId))
        {
            return Result.Fail<ProjectInvitation>(new DomainError("User is already a member of this project."));
        }

        if (_invitations.Any(x => x.UserId == userId && x.State == ProjectInvitationState.Pending))
        {
            return Result.Fail<ProjectInvitation>(new DomainError("There already exists a pending invitation for this user."));
        }

        DateTime? expirationDate = expirationDays.HasValue ? now.AddDays(expirationDays.Value) : null;
        var invitation = ProjectInvitation.Create(userId, Id, now, expirationDate);
        _invitations.Add(invitation);

        return invitation;
    }

    public Result<ProjectMember> AcceptInvitation(Guid invitationId, DateTime now)
    {
        var invitationResult = GetPendingInvitation(invitationId);
        if (invitationResult.IsFailed)
        {
            return Result.Fail<ProjectMember>(invitationResult.Errors);
        }

        var invitation = invitationResult.Value;
        invitation.Accept(now);
        return AddMember(invitation.UserId, RolesManager.GetReadOnlyRoleId());
    }

    public Result DeclineInvitation(Guid invitationId, DateTime now)
    {
        var invitationResult = GetPendingInvitation(invitationId);
        if (invitationResult.IsFailed)
        {
            return Result.Fail(invitationResult.Errors);
        }

        invitationResult.Value.Decline(now);
        return Result.Ok();
    }

    public Result CancelInvitation(Guid invitationId, DateTime now)
    {
        var invitationResult = GetPendingInvitation(invitationId);
        if (invitationResult.IsFailed)
        {
            return Result.Fail(invitationResult.Errors);
        }

        invitationResult.Value.Cancel(now);
        return Result.Ok();
    }

    public void ExpireInvitations(DateTime now)
    {
        foreach (var invitation in _invitations)
        {
            invitation.Expire(now);
        }
    }

    public Result RemoveMember(Guid memberId)
    {
        var member = _members.FirstOrDefault(x => x.Id == memberId);
        if (member is null)
        {
            return Result.Fail(new DomainError("Member with this ID does not exist."));
        }

        if (member.UserId == OwnerId)
        {
            return Result.Fail(new DomainError("Can't remove owner."));
        }


        _members.Remove(member);
        return Result.Ok();
    }

    private Result<ProjectInvitation> GetPendingInvitation(Guid invitationId)
    {
        var invitation = _invitations.FirstOrDefault(x => x.Id == invitationId);
        if (invitation is null)
        {
            return Result.Fail(new DomainError("Invitation with this ID does not exist."));
        }

        if (invitation.State != ProjectInvitationState.Pending)
        {
            return Result.Fail(new DomainError("Invitation is not in the correct state."));
        }

        return invitation;
    }

    private ProjectMember AddMember(Guid userId, Guid roleId)
    {
        var member = ProjectMember.Create(userId, roleId);
        _members.Add(member);
        return member;
    }
}
