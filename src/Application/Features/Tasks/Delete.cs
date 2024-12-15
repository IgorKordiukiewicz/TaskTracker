using Application.Common;
using Infrastructure.Extensions;
using Task = Domain.Tasks.Task;

namespace Application.Features.Tasks;

public record DeleteTaskCommand(Guid TaskId) : IRequest<Result>;

internal class DeleteTaskCommandValidator : AbstractValidator<DeleteTaskCommand>
{
    public DeleteTaskCommandValidator()
    {
        RuleFor(x => x.TaskId).NotEmpty();
    }
}

internal class DeleteTaskHandler(AppDbContext dbContext, ITasksBoardLayoutService tasksBoardLayoutService) 
    : IRequestHandler<DeleteTaskCommand, Result>
{
    public async Task<Result> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        var projectId = await dbContext.Tasks
            .Where(x => x.Id == request.TaskId)
            .Select(x => x.ProjectId)
            .FirstOrDefaultAsync(cancellationToken);
        if(projectId == default)
        {
            return Result.Fail(new NotFoundError<Task>(request.TaskId));
        }

        return await dbContext.ExecuteTransaction(async () =>
        {
            await dbContext.Tasks.DeleteAll(x => x.Id == request.TaskId, cancellationToken);
            await tasksBoardLayoutService.HandleChanges(projectId, 
                layout => layout.DeleteTask(request.TaskId), cancellationToken);
        });
    }
}
