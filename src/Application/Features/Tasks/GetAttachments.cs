namespace Application.Features.Tasks;

public record GetTaskAttachmentsQuery(Guid TaskId) : IRequest<Result<TaskAttachmentsVM>>;

internal class GetTaskAttachmentsQueryValidator : AbstractValidator<GetTaskAttachmentsQuery>
{
    public GetTaskAttachmentsQueryValidator()
    {
        RuleFor(x => x.TaskId).NotEmpty();
    }
}

internal class GetTaskAttachmentsHandler(AppDbContext dbContext)
    : IRequestHandler<GetTaskAttachmentsQuery, Result<TaskAttachmentsVM>>
{
    public async Task<Result<TaskAttachmentsVM>> Handle(GetTaskAttachmentsQuery request, CancellationToken cancellationToken)
    {
        if (!await dbContext.Tasks.AnyAsync(x => x.Id == request.TaskId, cancellationToken))
        {
            return Result.Fail<TaskAttachmentsVM>(new NotFoundError<Domain.Tasks.Task>(request.TaskId));
        }

        var attachments = await dbContext.TaskAttachments
            .Where(x => x.TaskId == request.TaskId)
            .Select(x => new
            {
                x.Name,
                x.BytesLength,
                x.Type
            })
            .ToListAsync();

        return Result.Ok(new TaskAttachmentsVM(attachments.Select(x => new TaskAttachmentVM(
            Name: x.Name, 
            BytesLength: x.BytesLength, 
            Type: x.Type, 
            DownloadUrl: string.Empty)).ToList()));
    }
}
