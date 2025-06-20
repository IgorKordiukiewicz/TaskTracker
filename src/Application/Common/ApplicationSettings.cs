namespace Application.Common;

public record ApplicationSettings
{
    public required BlobSettings Blob { get; init; }
}

public record BlobSettings
{
    public required string Container { get; init; }
    public required BlobPathsSettings Paths { get; init; }
}

public record BlobPathsSettings
{
    public required string TaskAttachments { get; init; }
}