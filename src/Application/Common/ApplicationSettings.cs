namespace Application.Common;

public record ApplicationSettings
{
    public required BlobPathsSettings BlobPaths { get; init; }
}

public record BlobPathsSettings
{
    public required string TaskAttachments { get; init; }
}