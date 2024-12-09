namespace Domain.Organizations;

public class Organization : Entity, IAggregateRoot, IHasName
{
    public string Name { get; set; } = string.Empty;

    public Guid OwnerId { get; private init; } // User


    private readonly List<OrganizationMember> _members = [];
    public IReadOnlyList<OrganizationMember> Members => _members.AsReadOnly();

    private readonly List<OrganizationInvitation> _invitations = [];
    public IReadOnlyList<OrganizationInvitation> Invitations => _invitations.AsReadOnly();

    private readonly List<OrganizationRole> _roles = [];
    public IReadOnlyList<OrganizationRole> Roles => _roles.AsReadOnly();

    public RolesManager<OrganizationRole, OrganizationPermissions> RolesManager { get; init; }

    private Organization(Guid id)
        : base(id)
    {
        RolesManager = new(_roles, (name, permissions) => new OrganizationRole(name, Id, permissions));
    }

    public static Organization Create(string name, Guid ownerId)
    {
        var result = new Organization(Guid.NewGuid())
        {
            Name = name,
            OwnerId = ownerId,
        };

        result._roles.AddRange(OrganizationRole.CreateDefaultRoles(result.Id));

        _ = result.AddMember(ownerId, result.RolesManager.GetOwnerRoleId()!.Value);

        return result;
    }

    public Result<OrganizationInvitation> CreateInvitation(Guid userId, DateTime now, int? expirationDays = null)
    {
        if(_members.Any(x => x.UserId == userId))
        {
            return Result.Fail<OrganizationInvitation>(new DomainError("User is already a member of this organization."));
        }

        if(_invitations.Any(x => x.UserId == userId && x.State == OrganizationInvitationState.Pending))
        {
            return Result.Fail<OrganizationInvitation>(new DomainError("There already exists a pending invitation for this user."));
        }

        DateTime? expirationDate = expirationDays.HasValue ? now.AddDays(expirationDays.Value) : null;
        var invitation = OrganizationInvitation.Create(userId, Id, now, expirationDate);
        _invitations.Add(invitation);

        return invitation;
    }

    public Result<OrganizationMember> AcceptInvitation(Guid invitationId, DateTime now)
    {
        var invitationResult = GetPendingInvitation(invitationId);
        if (invitationResult.IsFailed)
        {
            return Result.Fail<OrganizationMember>(invitationResult.Errors);
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
        foreach(var invitation in _invitations)
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

        if(member.UserId == OwnerId)
        {
            return Result.Fail(new DomainError("Can't remove owner."));
        }

        _members.Remove(member);
        return Result.Ok();
    }

    public Result Leave(Guid userId)
    {
        if(OwnerId == userId)
        {
            return Result.Fail(new DomainError("Owner can't leave the organization."));
        }

        var member = _members.First(x => x.UserId == userId);
        _members.Remove(member);

        return Result.Ok();
    }

    public IReadOnlyList<OrganizationMember> GetMemberManagers()
    {
        var editMembersRolesIds = _roles
            .Where(x => x.HasPermission(OrganizationPermissions.EditMembers))
            .Select(x => x.Id)
            .ToHashSet();

        return _members
            .Where(x => editMembersRolesIds.Contains(x.RoleId))
            .ToList();
    }

    private Result<OrganizationInvitation> GetPendingInvitation(Guid invitationId)
    {
        var invitation = _invitations.FirstOrDefault(x => x.Id == invitationId);
        if (invitation is null)
        {
            return Result.Fail(new DomainError("Invitation with this ID does not exist."));
        }

        if (invitation.State != OrganizationInvitationState.Pending)
        {
            return Result.Fail(new DomainError("Invitation is not in the correct state."));
        }

        return invitation;
    }

    private OrganizationMember AddMember(Guid userId, Guid roleId)
    {
        var member = OrganizationMember.Create(userId, roleId);
        _members.Add(member);
        return member;
    }
}