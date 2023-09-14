using Domain.Common;

namespace Domain.Tasks;

public class TaskStatus : Entity
{
    public string Name { get; private set; }

    public bool IsInitial { get; private set; } = false;

    private List<Guid> _possibleNextStatuses = new();
    public IReadOnlyList<Guid> PossibleNextStatuses => _possibleNextStatuses.AsReadOnly();

    public int DisplayOrder { get; private set; }

    private TaskStatus(Guid id, string name, int displayOrder, bool isInitial = false)
        : base(id)
    {
        Name = name;
        IsInitial = isInitial;
        DisplayOrder = displayOrder;
    }

    public static TaskStatus Create(Guid id, string name, IEnumerable<Guid> possibleNextStatuses, int displayOrder, bool isInitial = false)
    {
        var result = new TaskStatus(id, name, displayOrder, isInitial)
        {
            _possibleNextStatuses = possibleNextStatuses.ToList() // has to be initailized here because of EF
        };
        return result;
    }

    public bool CanTransitionTo(Guid statusId)
    {
        return _possibleNextStatuses.Contains(statusId);
    }
}