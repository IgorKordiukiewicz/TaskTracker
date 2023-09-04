using Domain.Common;

namespace Domain.Tasks;

public class Task : Entity, IAggregateRoot
{
    public int ShortId { get; private set; }
    public Guid ProjectId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    private Task(Guid id)
        : base(id)
    {

    }

    public static Task Create(int shortId, Guid projectId, string title, string description)
    {
        return new(Guid.NewGuid())
        {
            ShortId = shortId,
            ProjectId = projectId,
            Title = title,
            Description = description
        };
    }
}
