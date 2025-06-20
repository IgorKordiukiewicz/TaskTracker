using Infrastructure.Errors;
using Infrastructure.Services;
using Microsoft.Extensions.Options;

namespace Application.Features.Tasks;

public record DownloadTaskAttachmentQuery(Guid TaskId, string AttachmentName) : IRequest<Result<string>>;

internal class DownloadTaskAttachmentQueryValidator : AbstractValidator<DownloadTaskAttachmentQuery>
{
    public DownloadTaskAttachmentQueryValidator()
    {
        RuleFor(x => x.TaskId).NotEmpty();
        RuleFor(x => x.AttachmentName).NotEmpty();
    }
}

internal class DownloadTaskAttachmentHandler(AppDbContext dbContext, IBlobStorageService blobStorageService, IOptions<InfrastructureSettings> infrastructureSettings)
    : IRequestHandler<DownloadTaskAttachmentQuery, Result<string>>
{
    public async Task<Result<string>> Handle(DownloadTaskAttachmentQuery request, CancellationToken cancellationToken)
    {
        if(!await dbContext.TaskAttachments.AnyAsync(x => x.TaskId == request.TaskId && x.Name == request.AttachmentName, cancellationToken))
        {
            return Result.Fail<string>(new NotFoundError<Domain.Tasks.TaskAttachment>(request.AttachmentName));
        }

        var task = await dbContext.Tasks
            .Where(x => x.Id == request.TaskId)
            .Select(x => new { x.Id, x.ProjectId })
            .FirstAsync(cancellationToken);

        var path = string.Format(infrastructureSettings.Value.Blob.Paths.TaskAttachments, task.ProjectId, task.Id, request.AttachmentName);
        var downloadUrl = await blobStorageService.GetDownloadUrl(path);
        if(string.IsNullOrEmpty(downloadUrl))
        {
            return Result.Fail(new ApplicationError("Couldn't get download url for the attachment."));
        }

        return downloadUrl;
    }
}
