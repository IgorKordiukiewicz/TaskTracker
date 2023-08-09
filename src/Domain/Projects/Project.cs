using Domain.Common;
using FluentResults;

namespace Domain.Projects;

public class Project : Entity
{
    public string Name { get; private set; } = string.Empty;
    public Guid OrganizationId { get; private set; }

    private readonly List<ProjectMember> _members = new();
    public IReadOnlyList<ProjectMember> Members => _members.AsReadOnly();

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

        var member = ProjectMember.Create(createdByUserId);
        result._members.Add(member);

        return result;
    }

    public Result<ProjectMember> AddMember(Guid userId) // only allow to add members from org
    {
        if(_members.Any(x => x.UserId == userId))
        {
            return Result.Fail<ProjectMember>(new Error("User is already a member of this project."));
        }

        var member = ProjectMember.Create(userId);
        _members.Add(member);

        return member;
    }

    public Result<ProjectMember> RemoveMember(Guid memberId) // TODO: Automatically remove the member's row (override savechanges in dbContext/repository)
    {
        var member = _members.FirstOrDefault(x => x.Id == memberId);
        if (member is null)
        {
            return Result.Fail(new Error("Member with this ID does not exist."));
        }

        _members.Remove(member);
        return member;
    }
}
