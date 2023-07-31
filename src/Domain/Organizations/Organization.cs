using Domain.Common;
using FluentResults;

namespace Domain.Organizations;

// TODO:
// Organization has many projects, project has one organization
// Project has many project members (created from organization member)?
public class Organization : Entity<Guid>, IAggregateRoot
{
    public string Name { get; private set; } = string.Empty;

    public Guid OwnerId { get; private set; } // User


    private readonly List<OrganizationMember> _members = new();
    public IReadOnlyList<OrganizationMember> Members => _members.AsReadOnly();

    private readonly List<OrganizationInvitation> _invitations = new();
    public IReadOnlyList<OrganizationInvitation> Invitations => _invitations.AsReadOnly();

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

        var member = OrganizationMember.Create(ownerId);
        result._members.Add(member);

        return result;
    }

    public Result<OrganizationInvitation> CreateInvitation(Guid userId)
    {
        if(_members.Any(x => x.UserId == userId))
        {
            return Result.Fail<OrganizationInvitation>(new Error("User is already a member of this organization."));
        }

        if(_invitations.Any(x => x.UserId == userId && x.State == OrganizationInvitationState.Pending))
        {
            return Result.Fail<OrganizationInvitation>(new Error("There already exists a pending invitation for this user."));
        }
        
        var invitation = OrganizationInvitation.Create(userId, Id);
        _invitations.Add(invitation);

        return invitation;
    }
}