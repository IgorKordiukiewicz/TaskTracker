using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Task = Domain.Tasks.Task;

namespace Application.Features.Tasks;

public record AddTaskAttachmentCommand(Guid TaskId, IFormFile File) : IRequest<Result>;

internal class AddTaskAttachmentCommandValidator : AbstractValidator<AddTaskAttachmentCommand>
{
    public AddTaskAttachmentCommandValidator()
    {
        RuleFor(x => x.TaskId).NotEmpty();
        RuleFor(x => x.File).NotNull();
    }
}

internal class AddTaskAttachmentHandler(IRepository<Task> taskRepository, IBlobStorageService blobStorageService, IOptions<InfrastructureSettings> infrastructureSettings)
    : IRequestHandler<AddTaskAttachmentCommand, Result>
{
    public async Task<Result> Handle(AddTaskAttachmentCommand request, CancellationToken cancellationToken)
    {
        var task = await taskRepository.GetById(request.TaskId, cancellationToken);
        if (task is null)
        {
            return Result.Fail(new NotFoundError<Task>(request.TaskId));
        }

        var result = task.AddAttachment(request.File.FileName, request.File.Length, request.File.ContentType);
        if(result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        await blobStorageService.UploadFile(request.File, string.Format(
            infrastructureSettings.Value.Blob.Paths.TaskAttachments, task.ProjectId, task.Id, request.File.FileName)); // TODO: Move to infra?

        await taskRepository.Update(task, cancellationToken);
        return Result.Ok();
    }
}
