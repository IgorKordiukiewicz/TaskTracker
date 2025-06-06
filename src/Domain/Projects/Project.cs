﻿using Domain.Enums;

namespace Domain.Projects;

public class Project : Entity, IAggregateRoot
{
    public string Name { get; set; } = string.Empty;
    public Guid OwnerId { get; private init; }

    private readonly List<ProjectMember> _members = [];
    public IReadOnlyList<ProjectMember> Members => _members.AsReadOnly();

    private readonly List<ProjectInvitation> _invitations = [];
    public IReadOnlyList<ProjectInvitation> Invitations => _invitations.AsReadOnly();

    private readonly List<MemberRole> _roles = [];
    public IReadOnlyList<MemberRole> Roles => _roles.AsReadOnly();

    public RolesManager RolesManager { get; private init; }

    private Project(Guid id)
        : base(id)
    {
        RolesManager = new(_roles, id);
    }

    public static Project Create(string name, Guid ownerId)
    {
        var result = new Project(Guid.NewGuid())
        {
            Name = name,
            OwnerId = ownerId,
        };

        result._roles.AddRange(MemberRole.CreateDefaultRoles(result.Id));

        var member = ProjectMember.Create(ownerId, result.RolesManager.GetOwnerRoleId()!.Value);
        result._members.Add(member);
        result.AddEvent(new ProjectCreated(result.Id, ownerId, DateTime.UtcNow));

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
        AddEvent(new ProjectInvitationCreated(Id, userId, DateTime.UtcNow));

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
        AddEvent(new ProjectInvitationAccepted(Id, invitation.UserId, DateTime.UtcNow));

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
        AddEvent(new ProjectInvitationDeclined(Id, invitationResult.Value.UserId, DateTime.UtcNow));

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
        AddEvent(new ProjectInvitationCanceled(Id, invitationResult.Value.UserId, DateTime.UtcNow));

        return Result.Ok();
    }

    public void ExpireInvitations(DateTime now)
    {
        foreach (var invitation in _invitations)
        {
            var expired = invitation.Expire(now);

            if(expired)
            {
                AddEvent(new ProjectInvitationExpired(Id, invitation.UserId, DateTime.UtcNow));
            }
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
        AddEvent(new ProjectMemberRemoved(Id, member.UserId, DateTime.UtcNow));

        return Result.Ok();
    }

    public Result Leave(Guid userId)
    {
        if (OwnerId == userId)
        {
            return Result.Fail(new DomainError("Owner can't leave the project."));
        }

        var member = _members.First(x => x.UserId == userId);
        _members.Remove(member);
        AddEvent(new ProjectMemberLeft(Id, userId, DateTime.UtcNow));

        return Result.Ok();
    }

    public IReadOnlyList<ProjectMember> GetMembersWithEditMembersPermission()
    {
        var rolesIds = _roles
            .Where(x => x.HasPermission(ProjectPermissions.EditMembers))
            .Select(x => x.Id)
            .ToHashSet();

        return _members
            .Where(x => rolesIds.Contains(x.RoleId))
            .ToList();
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
