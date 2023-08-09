using Domain.Common;
using FluentResults;

namespace Domain.Projects;

public class Project : Entity<Guid>
{
    public string Name { get; private set; }
    public Guid OrganizationId { get; private set; }

    private readonly List<ProjectMember> _members = new();
    public IReadOnlyList<ProjectMember> Members => _members.AsReadOnly();

    private Project(Guid id, string name, Guid organizationId)
        : base(id)
    {
        Name = name;
        OrganizationId = organizationId;
    }

    public static Project Create(string name, Guid organizationId) // TODO: Add user who created it as member by default
    {
        return new(Guid.NewGuid(), name, organizationId);
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
}
