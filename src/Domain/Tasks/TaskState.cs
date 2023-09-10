using Domain.Common;

namespace Domain.Tasks;

public class TaskStateName : ValueObject
{
    public string Value { get; private init; }

    public TaskStateName(string value)
    {
        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value.ToLower();
    }
}

public class TaskState : Entity
{
    public TaskStateName Name { get; private set; }

    public bool IsInitial { get; private set; } = false;

    private List<Guid> _possibleNextStates = new();
    public IReadOnlyList<Guid> PossibleNextStates => _possibleNextStates.AsReadOnly();

    public int DisplayOrder { get; private set; }

    private TaskState(Guid id, TaskStateName name, int displayOrder, bool isInitial = false)
        : base(id)
    {
        Name = name;
        IsInitial = isInitial;
        DisplayOrder = displayOrder;
    }

    public static TaskState Create(Guid id, TaskStateName name, IEnumerable<Guid> possibleNextStates, int displayOrder, bool isInitial = false)
    {
        var result = new TaskState(id, name, displayOrder, isInitial)
        {
            _possibleNextStates = possibleNextStates.ToList() // has to be initailized here because of EF
        };
        return result;
    }

    public bool CanTransitionTo(Guid stateId)
    {
        return _possibleNextStates.Contains(stateId);
    }
}