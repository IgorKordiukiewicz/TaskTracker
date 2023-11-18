using Domain.Common;
using Domain.Errors;
using Domain.Organizations;
using FluentResults;
using Shared.Enums;

namespace Domain.Projects;

public class Project : Entity, IAggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public Guid OrganizationId { get; private set; }

    private readonly List<ProjectMember> _members = new();
    public IReadOnlyList<ProjectMember> Members => _members.AsReadOnly();

    private readonly List<ProjectRole> _roles = new();
    public IReadOnlyList<ProjectRole> Roles => _roles.AsReadOnly();

    private Project(Guid id)
        : base(id)
    {
    }

    public static Project Create(string name, Guid organizationId, Guid createdByUserId)
    {
        var result = new Project(Guid.NewGuid())
        {
            Name = name,
            OrganizationId = organizationId,
        };

        result._roles.AddRange(ProjectRole.CreateDefaultRoles(result.Id));

        var member = ProjectMember.Create(createdByUserId, result._roles.GetAdminRoleId());
        result._members.Add(member);

        return result;
    }

    public Result<ProjectMember> AddMember(Guid userId)
    {
        if(_members.Any(x => x.UserId == userId))
        {
            return Result.Fail<ProjectMember>(new DomainError("User is already a member of this project."));
        }

        var member = ProjectMember.Create(userId, _roles.GetReadOnlyRoleId());
        _members.Add(member);

        return member;
    }

    public Result RemoveMember(Guid memberId)
    {
        var member = _members.FirstOrDefault(x => x.Id == memberId);
        if (member is null)
        {
            return Result.Fail(new DomainError("Member with this ID does not exist."));
        }

        _members.Remove(member);
        return Result.Ok();
    }

    public Result AddRole(string name, ProjectPermissions permissions)
        => _roles.AddRole<ProjectPermissions, ProjectRole>(new ProjectRole(name, Id, permissions));

    // RemoveRole: check if is used by any member and don't remove default role,
    // Add interface to ProjectMember & OrganizationMember: IHasRole and then RolesService/Manager can accept a list of IHasRole objects to check whether any use a given role ?
}
