namespace Domain.Tasks;

public class TaskTimeLog : ValueObject
{
    public Guid TaskId { get; private init; }
    public int Minutes { get; private init; }
    public DateOnly Day { get; private init; }
    public Guid LoggedBy { get; private init; }

    public TaskTimeLog(Guid taskId, int minutes, DateOnly day, Guid loggedBy)
    {
        TaskId = taskId;
        Minutes = minutes;
        Day = day;
        LoggedBy = loggedBy;
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return TaskId;
        yield return Minutes;
        yield return Day;
        yield return LoggedBy;
    }
}