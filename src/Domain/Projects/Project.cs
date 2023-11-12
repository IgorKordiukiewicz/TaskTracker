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

        result._roles.Add(new ProjectRole("Administrator", result.Id, ProjectPermissions.Tasks | ProjectPermissions.Members | ProjectPermissions.Workflows)); // TODO: Create All enum field ?
        result._roles.Add(new ProjectRole("Read-Only", result.Id, ProjectPermissions.None));

        var member = ProjectMember.Create(createdByUserId, result._roles.Single(x => x.IsAdminRole()).Id);
        result._members.Add(member);

        return result;
    }

    public Result<ProjectMember> AddMember(Guid userId)
    {
        if(_members.Any(x => x.UserId == userId))
        {
            return Result.Fail<ProjectMember>(new DomainError("User is already a member of this project."));
        }

        var member = ProjectMember.Create(userId, _roles.Single(x => x.IsReadOnlyRole()).Id);
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
}
