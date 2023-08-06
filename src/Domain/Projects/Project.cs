using Domain.Common;

namespace Domain.Projects;

public class Project : Entity<Guid>
{
    public string Name { get; private set; }
    public Guid OrganizationId { get; private set; }

    private Project(Guid id, string name, Guid organizationId)
        : base(id)
    {
        Name = name;
        OrganizationId = organizationId;
    }

    public static Project Create(string name, Guid organizationId)
    {
        return new(Guid.NewGuid(), name, organizationId);
    }
}
