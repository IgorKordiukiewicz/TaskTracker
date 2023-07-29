using Domain.Common;

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
}