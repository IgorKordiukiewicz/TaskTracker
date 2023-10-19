using Domain.Common;

namespace Domain.Workflows;

public class TaskStatus : Entity
{
    public string Name { get; private set; }

    public bool Initial { get; private set; } = false;

    public int DisplayOrder { get; private set; }
    // TODO: Add color

    private TaskStatus(Guid id, string name, int displayOrder, bool initial = false)
        : base(id)
    {
        Name = name;
        Initial = initial;
        DisplayOrder = displayOrder;
    }

    // TODO: Ctor instead of factory method
    public static TaskStatus Create(Guid id, string name, int displayOrder, bool initial = false)
    {
        return new TaskStatus(id, name, displayOrder, initial);
    }
}