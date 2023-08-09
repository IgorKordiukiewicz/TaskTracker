using Domain.Common;

namespace Domain.Projects;

public class ProjectMember : Entity
{
    public Guid UserId { get; private init; }

    private ProjectMember(Guid id) 
        : base(id)
    {
    }

    public static ProjectMember Create(Guid userId)
    {
        return new(Guid.NewGuid())
        {
            UserId = userId,
        };
    }
}
