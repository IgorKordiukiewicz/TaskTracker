using Domain.Common;
using Domain.Errors;
using Domain.Projects;
using FluentResults;
using Shared.Enums;

namespace Domain.Organizations;

public class Organization : Entity, IAggregateRoot
{
    public string Name { get; private set; } = string.Empty;

    public Guid OwnerId { get; private set; } // User


    private readonly List<OrganizationMember> _members = new();
    public IReadOnlyList<OrganizationMember> Members => _members.AsReadOnly();

    private readonly List<OrganizationInvitation> _invitations = new();
    public IReadOnlyList<OrganizationInvitation> Invitations => _invitations.AsReadOnly();

    private readonly List<OrganizationRole> _roles = new();
    public IReadOnlyList<OrganizationRole> Roles => _roles.AsReadOnly();

    private Organization(Guid id)
        : base(id)
    {

    }

    public static Organization Create(string name, Guid ownerId)
    {
        var result = new Organization(Guid.NewGuid())
        {
            Name = name,
            OwnerId = ownerId,
        };

        result._roles.AddRange(OrganizationRole.CreateDefaultRoles(result.Id));

        _ = result.AddMember(ownerId, result._roles.GetAdminRoleId());

        return result;
    }

    public Result<OrganizationInvitation> CreateInvitation(Guid userId)
    {
        if(_members.Any(x => x.UserId == userId))
        {
            return Result.Fail<OrganizationInvitation>(new DomainError("User is already a member of this organization."));
        }

        if(_invitations.Any(x => x.UserId == userId && x.State == OrganizationInvitationState.Pending))
        {
            return Result.Fail<OrganizationInvitation>(new DomainError("There already exists a pending invitation for this user."));
        }
        
        var invitation = OrganizationInvitation.Create(userId, Id);
        _invitations.Add(invitation);

        return invitation;
    }

    public Result<OrganizationMember> AcceptInvitation(Guid invitationId)
    {
        var invitationResult = GetPendingInvitation(invitationId);
        if (invitationResult.IsFailed)
        {
            return Result.Fail<OrganizationMember>(invitationResult.Errors);
        }

        var invitation = invitationResult.Value;
        invitation.Accept();
        return AddMember(invitation.UserId, _roles.GetReadOnlyRoleId());
    }

    public Result DeclineInvitation(Guid invitationId)
    {
        var invitationResult = GetPendingInvitation(invitationId);
        if (invitationResult.IsFailed)
        {
            return Result.Fail(invitationResult.Errors);
        }

        invitationResult.Value.Decline();
        return Result.Ok();
    }

    public Result CancelInvitation(Guid invitationId)
    {
        var invitationResult = GetPendingInvitation(invitationId);
        if (invitationResult.IsFailed)
        {
            return Result.Fail(invitationResult.Errors);
        }

        invitationResult.Value.Cancel();
        return Result.Ok();
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