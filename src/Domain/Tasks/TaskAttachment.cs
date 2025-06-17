namespace Domain.Tasks;

public class TaskAttachment : ValueObject
{
    public Guid TaskId { get; private init; }
    public string Name { get; private init; }
    public long BytesLength { get; private init; }
    public AttachmentType Type { get; private init; }

    public TaskAttachment(Guid taskId, string name, long bytesLength, AttachmentType type)
    {
        TaskId = taskId;
        Name = name;
        BytesLength = bytesLength;
        Type = type;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return TaskId;
        yield return Name;
        yield return BytesLength;
        yield return Type;
    }

    // TODO: Add zip support for attachments (AttachmentType.Archive)
    public static AttachmentType GetAttachmentType(string contentType)
        => contentType switch
        {
            "application/pdf" or 
            "application/json" or
            "text/plain" or
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document" or
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" or
            "application/vnd.openxmlformats-officedocument.presentationml.presentation" or
            "text/csv" => AttachmentType.Document,
            "image/jpeg" or 
            "image/png" or 
            "image/gif" => AttachmentType.Image,
            _ => throw new ArgumentException("Unsupported content type", nameof(contentType))
        };
}
