namespace Domain.Tasks;

public class TaskComment : Entity
{
    public Guid TaskId { get; private set; }
    public string Content { get; private set; }
    public Guid AuthorId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public TaskComment(Guid taskId, string content, Guid authorId, DateTime createdAt) 
        : base(Guid.NewGuid())
    {
        TaskId = taskId;
        Content = content;
        AuthorId = authorId;
        CreatedAt = createdAt;
    }
}
